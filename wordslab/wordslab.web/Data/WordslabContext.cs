using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using wordslab.web.Models;
using wordslab.web.Models.Data;

namespace wordslab.web.Data
{
    public class WordslabContext : DbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public WordslabContext(DbContextOptions<WordslabContext> options, IHttpContextAccessor httpContextAccessor)
               : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<DataSet> DataSets { get; set; }

        public DbSet<AuditRecord> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditRecord>().ToTable("AuditLog");
        }

        // 
        // --- https://dejanstojanovic.net/aspnet/2018/november/tracking-data-changes-with-entity-framework-core/ ---

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotSupportedException("Please use SaveChangesAsync");
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entitiesWithTemporaryProperties = await AuditNonTemporaryProperties();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await AuditTemporaryProperties(entitiesWithTemporaryProperties);
            return result;
        }

        async Task<IEnumerable<Tuple<EntityEntry, AuditRecord>>> AuditNonTemporaryProperties()
        {
            ChangeTracker.DetectChanges();
            var entitiesToTrack = ChangeTracker.Entries().Where(e => e.Entity is AuditableEntity && e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            await AuditLog.AddRangeAsync(
                entitiesToTrack.Where(e => !e.Properties.Any(p => p.IsTemporary)).Select(e => BuildAuditRecord(e)).ToList()
            );

            // Return list of pairs of EntityEntry and AuditRecord  
            return entitiesToTrack.Where(e => e.Properties.Any(p => p.IsTemporary))
                 .Select(e => new Tuple<EntityEntry, AuditRecord>(e, BuildAuditRecord(e, addGenerated: false))).ToList();
        }
                
        async Task AuditTemporaryProperties(IEnumerable<Tuple<EntityEntry, AuditRecord>> entitiesWithTemporaryProperties)
        {
            if (entitiesWithTemporaryProperties != null && entitiesWithTemporaryProperties.Any())
            {
                await AuditLog.AddRangeAsync(
                entitiesWithTemporaryProperties.ForEach(t => t.Item2.EntityKey = t.Item1.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue?.ToString()).FirstOrDefault())
                    .Select(t => t.Item2)
                );
                await base.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }

        private AuditRecord BuildAuditRecord(EntityEntry e, bool addGenerated = true)
        {
            return new AuditRecord()
            {
                EntityTypeName = ((AuditableEntity)e.Entity).EntityTypeName,
                EntityKey = addGenerated ? e.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue?.ToString()).FirstOrDefault() : null,
                UserLogin = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system",
                Timestamp = DateTime.Now.ToUniversalTime(),
                ChangeType = e.State switch { EntityState.Added => ChangeType.Created, EntityState.Modified => ChangeType.Updated, EntityState.Deleted => ChangeType.Deleted },
                ValuesBefore = e.Properties.Where(p => e.State == EntityState.Deleted || e.State == EntityState.Modified).ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()).NullIfEmpty(),
                ValuesAfter = e.Properties.Where(p => e.State == EntityState.Added || e.State == EntityState.Modified).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()).NullIfEmpty()
            };
        }

    }
}
