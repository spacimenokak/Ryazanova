using Microsoft.AspNetCore.Mvc;

namespace projectTest.Controllers;

[ApiController]
[Route("product")]

public class ProductController : ControllerBase
{
  [HttpGet]
  public ActionResult Get()
  {
    return Ok("Hello World!");
  }

    [HttpPost]
  public ActionResult Post()
  {
    return Ok("Hello World!");
  }

    [HttpPut]
  public ActionResult Put()
  {
    return Ok("Hello World!");
  }

      [HttpDelete]
  public ActionResult Delete()
  {
    return Ok("DELETE WORLD!!!!!1");
    }
}