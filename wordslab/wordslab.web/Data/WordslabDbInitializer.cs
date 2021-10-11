using wordslab.web.Models.Data;

namespace wordslab.web.Data
{
    internal class WordslabDbInitializer
    {
        internal static async Task InitializeAsync(WordslabContext context)
        {
            // Look for any datasets
            if (context.DataSets.Any())
            {
                return;   // DB has been seeded
            }

            var datasets = new DataSet[]
            {
                new DataSet { Name = "Web pages from banking website" },
                new DataSet { Name = "Web pages from insurance website" }
            };
            context.DataSets.AddRange(datasets);

            await context.SaveChangesAsync();
        }
    }
}
