using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSite.Models;
using TestSite.Models.Comments;
using TestSite.ViewModels;

namespace TestSite.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.ToList();
        }

        public IndexPageViewModel GetAllPosts(int pageNumber, string category, string search)
        {
            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };

            var pageSize = 6;
            var skipAmount = pageSize * (pageNumber - 1);
            var capacity = skipAmount + pageNumber;

            var query = _context.Posts/*.AsNoTracking()*/.AsQueryable();
                    

            if (!string.IsNullOrEmpty(category))
                query = query.Where(post => post.Category.ToLower().Equals(category.ToLower()));

            if (!string.IsNullOrEmpty(search))
            {
                //query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                //    || EF.Functions.Like(x.Body, $"%{search}%")
                //    || EF.Functions.Like(x.Tags, $"%{search}%"));
                query = query.Where(x => x.Title.Contains(search) ||
                x.Body.Contains(search));
            }

            int postsCount = query.Count();

            return new IndexPageViewModel
            {
                PageNumber = pageNumber,
                PageCount = (int)Math.Ceiling(postsCount * 1.0 / pageSize),
                CanGoNextPage = postsCount > capacity,
                Category = category,
                Search = search,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };

            //return _context.Posts
            //    .Where(post => post.Category.ToLower().Equals(category.ToLower()))
            //    .ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(comments => comments.SubComments)
                .FirstOrDefault(_ => _.Id == id);
        }

        public void RemovePost(int id)
        {
            _context.Remove(GetPost(id));
        }
         
        public void UpdatePost(Post post)
        {
            _context.Update(post);
        }
        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
                return true;
            return false;
        }

        public void AddSubComment(SubComment subComment)
        {
            _context.SubComments.Add(subComment);
        }
    }
}
