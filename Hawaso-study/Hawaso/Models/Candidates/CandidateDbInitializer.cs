namespace Hawaso.Models.Candidates
{
    public static class CandidateDbInitializer
    {
        public static void Initialize(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var candidateDbContext = services.GetRequiredService<CandidateAppDbContext>();

                if (!candidateDbContext.Candidates.Any())
                {
                    candidateDbContext.Candidates.Add(new Candidate
                    {
                        FirstName = "꺽정",
                        LastName = "임",
                        IsEntollment = false,
                    });                    

                    candidateDbContext.SaveChanges();
                }
            }

        }

    }
}
