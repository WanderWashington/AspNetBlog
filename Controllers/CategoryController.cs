using AspNetBlog.Data;
using AspNetBlog.Extensions;
using AspNetBlog.Models;
using AspNetBlog.ViewModels;
using AspNetBlog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetBlog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> Get(
            [FromServices] IMemoryCache cache,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE4 - Não foi possível incluir a categoria");
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>(error: "05XE5 - Falha interna no servidor"));
            }
        }

        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList();
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>(error:"Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE6 - Não foi possível incluir a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>(error: "05XE7 - Falha interna no servidor"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower()
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE9 - Não foi possível incluir a categoria"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE10 - Falha interna no servidor"));
            }
           
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PostAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));
                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE11 - Não foi possível atualizar a categoria"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE12 - Falha interna no servidor"));
            }
        }
        [HttpDelete]
        [Route("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute]int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound();

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE13 - Não foi possível excluir a categoria"));
            }
            catch (Exception e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE14 - Falha interna no servidor"));
            }
        }
    }
}
