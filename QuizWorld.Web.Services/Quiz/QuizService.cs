using QuizWorld.Infrastructure;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.Quiz
{
    /// <summary>
    /// A service for interacting with quizzes in the database.
    /// </summary>
    public class QuizService : IQuizService
    {
        private readonly IRepository repository;
        public QuizService(IRepository repository)
        {
            this.repository = repository;
        }

        public Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateQuiz(CreateQuizViewModel quiz, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteQuizById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> EditQuizById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetAllQuizzes(int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<QuizViewModel> GetQuizById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetQuizzesByQuery(string query, int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(string userId, int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(Guid userId, int page, string category, string order)
        {
            throw new NotImplementedException();
        }
    }
}
