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
    public interface IRepository
    {
        Post GetPost(int id);
        List<Post> GetAllPosts();
        IndexPageViewModel GetAllPosts(int pageNumber, string category, string search);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);

        void AddSubComment(SubComment subComment);

        Task<bool> SaveChangesAsync();
    }
}
