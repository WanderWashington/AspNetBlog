using AspNetBlog.Data;
using AspNetBlog.Models;
using AspNetBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetBlog.Controllers
{
    public class PostController : ControllerBase
    {
        [HttpGet("v1/Posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X05 - Falha interna do servidor"));
            }
        }

        [HttpGet("v1/Posts/category/{category}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] string category,
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25)
         {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x=> x.Category.Slug == category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X05 - Falha interna do servidor"));
            }
        }


        [HttpGet("v1/Posts/{id:int}")]
        public async Task<IActionResult> GetAsync(
           [FromServices] BlogDataContext context,
           [FromRoute] int id)
        {
            try
            {
                var post = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x=> x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x=> x.Id == id);
                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));
                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X06 - Falha interna do servidor"));
            }
        }
    }
}
