using AzureApi.Models;
using AzureApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Configuration;

namespace AzureApi.Repository
{
    public class PostRepository : IPostRepository
    {
        BlogDBContext db;
        public PostRepository(BlogDBContext _db)
        {
            db = _db;
        }

        [HttpGet]
        public async Task<List<Category>> GetCategories()
        {
            if (db != null)
            {
                return await db.Category.ToListAsync();
            }

            return null;
        }

        [HttpGet]
        public async Task<List<PostViewModel>> GetPosts()
        {
            if (db != null)
            {
                return await (from p in db.Post
                              from c in db.Category
                              where p.CategoryId == c.Id
                              select new PostViewModel
                              {
                                  PostId = p.PostId,
                                  Title = p.Title,
                                  Description = p.Description,
                                  CategoryId = p.CategoryId,
                                  CategoryName = c.Name,
                                  CreatedDate = p.CreatedDate
                              }).ToListAsync();
            }

            return null;
        }

        [HttpGet]
        public async Task<PostViewModel> GetPost(int? postId)
        {
            if (db != null)
            {
                return await (from p in db.Post
                              from c in db.Category
                              where p.PostId == postId
                              select new PostViewModel
                              {
                                  PostId = p.PostId,
                                  Title = p.Title,
                                  Description = p.Description,
                                  CategoryId = p.CategoryId,
                                  CategoryName = c.Name,
                                  CreatedDate = p.CreatedDate
                              }).FirstOrDefaultAsync();
            }

            return null;
        }

        [HttpPost]
        public async Task<int> AddPost(Post post)
        {
            if (db != null)
            {
              /*  // -- ** Queue ** --
                var connString = ConfigurationManager.ConnectionStrings["MyStorageConnString"].ToString();
                
                //Queue is part of storge account
                var storageAccount = CloudStorageAccount.Parse(connString);

                // create Queue Client
                var client = storageAccount.CreateCloudQueueClient();

                // get Queue reference
                var queue = client.GetQueueReference("myqueue");

                // create Queue if it doesn't yet exist
                queue.CreateIfNotExists();


                // Create new Message
                var message = new CloudQueueMessage("Hello Azure");

                // Add / Send Message to the Queue
                queue.AddMessage(message);


                //// retrieve next Message from Queue
                var message1 = queue.PeekMessage();

                // get message contents
                var contents = message1.AsString;

                // -- End Queue ** --
                */
                await db.Post.AddAsync(post);
                await db.SaveChangesAsync();

                return post.PostId;
            }

            return 0;
        }

        [HttpDelete]
        public async Task<int> DeletePost(int? postId)
        {
            int result = 0;

            if (db != null)
            {
                //Find the post for specific post id
                var post = await db.Post.FirstOrDefaultAsync(x => x.PostId == postId);

                if (post != null)
                {
                    //Delete that post
                    db.Post.Remove(post);

                    //Commit the transaction
                    result = await db.SaveChangesAsync();
                }
                return result;
            }

            return result;
        }


        [HttpPut]
        public async Task UpdatePost(Post post)
        {
            if (db != null)
            {
                //Delete that post
                db.Post.Update(post);

                //Commit the transaction
                await db.SaveChangesAsync();
            }
        }
    }
}