using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Quiz;
using System.Linq.Expressions;

namespace QuizWorld.Web.Services
{
    public class QuizService(IRepository quizRepository) : IQuizService
    {
        private readonly IRepository _quizRepository = quizRepository;
        public async Task<CatalogueQuizViewModel> Search(QuizSearchParameterss parameters)
        {
            Expression<Func<Quiz, bool>> predicate = BuildPredicate(parameters);

            IQueryable<Quiz> query = _quizRepository
                    .AllReadonly(predicate);
            int count = await query.CountAsync();

            var quizzes = await query
                .Paginate(parameters.Page, parameters.PageSize)
                .SortByOptions(
                    category: parameters.SortBy, order: parameters.Order)
                .Select(q => new CatalogueQuizItemViewModel()
                {
                    Title = q.Title,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn,
                    Id = q.Id,
                    InstantMode = q.InstantMode,
                    Description = q.Description,
                })
                .ToListAsync();

            return new CatalogueQuizViewModel()
            {
                Total = count,
                Quizzes = quizzes,
            };
        }

        private static Expression<Func<Quiz, bool>> BuildPredicate(QuizSearchParameterss parameters)
        {
            if (parameters.Author != string.Empty)
            {
                return q => !q.IsDeleted
                && q.NormalizedTitle.Contains(parameters.Title.ToLower())
                && q.Creator.NormalizedUserName == parameters.Author.ToUpper();
            }
            else
            {
                return q => !q.IsDeleted 
                && q.NormalizedTitle.Contains(parameters.Title.ToLower());
            }
        }
    }
}