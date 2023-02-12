using DatingApp.Application.DTO;
using DatingApp.Application.DTO.Comments;
using DatingApp.Application.DTO.Likes;
using DatingApp.Application.DTO.Posts;
using DatingApp.Application.Interfaces;
using DatingApp.Core.Entities;
using DatingApp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Infrastructure.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;

        public PostService(IPostRepository postRepository, ILikeRepository likeRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }

        public async Task<AddPostResponse> CreateNewPost(AddPostRequest request)
        {
            string nameOfPostPicture = string.Empty;
            if (request.Image != null)
            {
                var folderSave = Path.Combine("/app/Share", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderSave);
                nameOfPostPicture = Guid.NewGuid().ToString() + "-" + request.Image.FileName;
                var fullPath = Path.Combine(pathToSave, nameOfPostPicture);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                request.Image.CopyTo(stream);
                }
            }

            PostUser newPost = new PostUser();
            newPost.Text = request.Text;
            newPost.Image = nameOfPostPicture;
            newPost.UserId = request.UserId;
            newPost.CreatedDate = DateTime.Now;
            await _postRepository.Add(newPost);

            return new AddPostResponse {
                Id = newPost.PostId,
                Text = newPost.Text,
                Image = newPost.Image,
                UserId = newPost.UserId,
                CreatedDate = newPost.CreatedDate,
            };
        }

        public async Task DeletePost(DeletePostRequest request)
        {
            await _postRepository.DeleteAllComment(request.Id);
            await _postRepository.DeleteAllLike(request.Id);

            PostUser postToDelete = await _postRepository.GetById(request.Id);
            await _postRepository.Delete(postToDelete);
        }

        public async Task<List<GetUserPostResponse>> GetAllPostOfUser(GetUserPostRequest request)
        {
            var postUsers =  await _postRepository.GetAll(request.UserId);

            return postUsers.Select(post => new GetUserPostResponse() {
                Id = post.PostId,
                UserId = post.UserId,
                Text = post.Text,
                Image = post.Image,
                CreatedDate = post.CreatedDate,
            }).ToList();
        }

        public List<GetUserPostResponse> GetAllPostOfUserAndFriend(GetUserPostRequest request)
        {
            var postUsers = _postRepository.GetAllAndFriend(request.UserId);

            return postUsers.Select(post => new GetUserPostResponse()
            {
                Id = post.PostId,
                UserId = post.UserId,
                Text = post.Text,
                Image = post.Image,
                CreatedDate = post.CreatedDate,
            }).ToList();
        }

        public async Task<GetPostResponse> GetPostById(GetPostRequest request)
        {
            PostUser postUser = await _postRepository.GetById(request.Id);

            return new GetPostResponse()
            {
                Id = postUser.PostId,
                Text = postUser.Text,
                Image = postUser.Image,
                UserId = postUser.UserId,
                CreatedDate = postUser.CreatedDate,
            };
        }

        public async Task<GetLikeNumberResponse> CountLikeOfPost(int id)
        {
            var likesOfPost = await _likeRepository.GetAll(id);
            return new GetLikeNumberResponse
            {
                LikesOfPostNumber = likesOfPost.Count(),
            };
        }

        public async Task<List<GetCommentResponse>> GetAllCommentOfAPost(int postId)
        {
            var commentsOfAPost = await _commentRepository.GetAll();

            return commentsOfAPost.Where(c => c.PostId == postId).Select(c => new GetCommentResponse
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                Text = c.Text,
            }).ToList();
        }
    }
}
