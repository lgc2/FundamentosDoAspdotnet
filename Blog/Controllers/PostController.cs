using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var countPages = (int)Math.Ceiling((double)count / pageSize);
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    //.Select(x => new { x.Id, x.Title }) // caso não tivesse um ViewModel, poderia ser feito dessa maneira
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(x => x.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    totalPages = countPages,
                    page = page + 1,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id)
        {
            try
            {
                var post = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (post is null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Post>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromRoute] string category,
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context
                    .Posts
                    .AsNoTracking()
                    .Where(x => x.Category.Slug == category)
                    .CountAsync();
                var countPages = (int)Math.Ceiling((double)count / pageSize);

                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x => x.Category.Slug == category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(x => x.Id)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (posts.Count == 0)
                    return NotFound(new ResultViewModel<Post>("Conteúdo(s) não encontrado(s)"));

                return Ok(new ResultViewModel<dynamic>(new
                {
                    totalPages = countPages,
                    page = page + 1,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05X04 - Falha interna no servidor"));
            }
        }

        [Authorize]
        [HttpGet("v1/posts/user-posts")]
        public async Task<IActionResult> GetUserPostsAsync([FromServices] BlogDataContext context)
        {
            try
            {
                var user = await context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

                if (user is null)
                    return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

                var userPosts = await context
                    .Posts
                    .AsNoTracking()
                    .Where(p => p.Author.Id == user.Id)
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                if (userPosts.Count == 0)
                    return Ok(new ResultViewModel<string>("Você ainda não publicou um Post."));


                return Ok(new ResultViewModel<List<Post>>(userPosts));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }
    }
}
