using AspNetBlog.Data;
using AspNetBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetBlog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> Get(
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE4 - Não foi possível incluir a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, "05XE5 - Falha interna no servidor");
            }
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
                    return NotFound();

                return Ok(category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE6 - Não foi possível incluir a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, "05XE7 - Falha interna no servidor");
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] Category category,
            [FromServices] BlogDataContext context)
        {
            try
            {
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", category);
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, "05XE9 - Não foi possível incluir a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, "05XE10 - Falha interna no servidor");
            }
           
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PostAsync(
            [FromRoute] int id,
            [FromBody] Category model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound();
                category.Name = model.Name;
                category.Slug = model.Slug;
                category.Posts = model.Posts;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE11 - Não foi possível atualizar a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, "05XE12 - Falha interna no servidor");
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
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05XE13 - Não foi possível excluir a categoria");
            }
            catch (Exception e)
            {
                return StatusCode(500, "05XE14 - Falha interna no servidor");
            }
        }
    }
}
