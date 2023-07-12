using QuizWorld.ViewModels.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Contracts.Quiz
{
    public interface IQuizService
    {
        Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId);
        Task<int> CreateQuiz(CreateQuizViewModel quiz, Guid userId);
        Task<QuizViewModel?> GetQuizById(int id);

        Task<int?> DeleteQuizById(int id);
        Task<int?> EditQuizById(int id, EditQuizViewModel quiz);
        Task<IEnumerable<CatalogueQuizViewModel>> GetAllQuizzes(int page, string category, string order);
        Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(string userId, int page, string category, string order);
        Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(Guid userId, int page, string category, string order);
        Task<IEnumerable<CatalogueQuizViewModel>> GetQuizzesByQuery(string query, int page, string category, string order);
    }
}
