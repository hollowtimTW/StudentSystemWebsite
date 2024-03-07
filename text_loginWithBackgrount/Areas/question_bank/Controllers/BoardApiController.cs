using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using text_loginWithBackgrount.Areas.question_bank.ViewModels;
using text_loginWithBackgrount.Services;


namespace 公佈欄_Mongodb.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BoardApiController : ControllerBase
	{
		private readonly BoardService _boardService;

		public BoardApiController(BoardService boardService) =>
		   _boardService = boardService;


		[HttpGet]
		public async Task<List<Board>> Get() =>
		   await _boardService.GetAsync();

		[HttpGet("{id:length(24)}")]
		public async Task<ActionResult<Board>> Get(string id)
		{
			var board = await _boardService.GetAsync(id);

			if (board is null)
			{
				return NotFound();
			}

			return board;
		}

		[HttpPost]
		public async Task<IActionResult> Post(Board newBoard)
		{
			await _boardService.CreateAsync(newBoard);

			return CreatedAtAction(nameof(Get), new { id = newBoard.Id }, newBoard);
		}

		[HttpPut("{id:length(24)}")]
		public async Task<IActionResult> Update(string id, Board updatedBoard)
		{
			var board = await _boardService.GetAsync(id);

			if (board is null)
			{
				return NotFound();
			}

			updatedBoard.Id = board.Id;

			await _boardService.UpdateAsync(id, updatedBoard);

			return NoContent();
		}

		[HttpDelete("{id:length(24)}")]
		public async Task<IActionResult> Delete(string id)
		{
			var board = await _boardService.GetAsync(id);

			if (board is null)
			{
				return NotFound();
			}

			await _boardService.RemoveAsync(id);

			return NoContent();
		}

	}
}
