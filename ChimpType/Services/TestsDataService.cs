using ChimpType.Data;
using Microsoft.EntityFrameworkCore;

namespace ChimpType.Services
{
    public class TestsDataService
    {
        private readonly ChimpTypeDbContext _context;

        public TestsDataService(ChimpTypeDbContext context) => _context = context;
        
        public async Task<List<TestsTaken>> GetTestByUsername(string userName)
        {
            var result = _context.TestsTakens.Where(x => x.User != null && x.User.Username == userName).ToList();
            return result;
        }

        public async Task<List<TestsTaken>> GetTestByUserId(string id)
        {
            var result = _context.TestsTakens.Where(x => x.UserId != null && x.UserId.ToString() == id).ToList();
            return result;
        }

        public async Task<List<TestsTaken>> GetTop(int topAmount, TopCategory topCategory)
        {
            var result = new List<TestsTaken>();
            switch (topCategory)
            {
                case TopCategory.Wpm:
                    result = _context.TestsTakens.Include(x => x.User).OrderByDescending(x => x.Wpm).Take(topAmount).ToList();
                    break;
                case TopCategory.Accuracy:
                    result = _context.TestsTakens.Include(x => x.User).OrderByDescending(x => x.Accuracy).Take(topAmount).ToList();
                    break;
            }
            return result;
        }

        public async Task<TestsTaken> InsertTestResult(string username, int wpm, double accuracy, string testType, int mistakes, int correctChars, int missedChars, int wrongChars, int extraChars, int totalTime)
        {
            var userId = _context.Users.FirstOrDefault(x => x.Username == username).Id;
            TestsTaken test = new()
            {
                TakenOn = DateTime.UtcNow,
                TestType = testType,
                UserId = userId,
                Wpm = wpm,
                Accuracy = Convert.ToDecimal(accuracy),
                CorrectCharacters = correctChars,
                ExtraCharacters = extraChars,
                MissedCharacters = missedChars,
                WrongCharacters = wrongChars,
                MistakesNumber = mistakes,
                TotalTime = totalTime,
            };

            _context.TestsTakens.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }
    }

    public enum TopCategory
    {
        Wpm,
        Accuracy
    }
}
