﻿using Microsoft.AspNetCore.Mvc;
using VVS_biblioteka.Models;

namespace VVS_biblioteka.Controllers
{
    [ApiController]
    [Route("bookController")]
    public class BookController : ControllerBase
    {
        private readonly LibDbContext _context;

        public BookController(LibDbContext context) 
        {
            _context = context;
        }

        [HttpPost]
        [Route("addBook")]
        public async Task<IActionResult> AddBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Book.Any(b => b.Id == book.Id))
            {
                return BadRequest("Book with the same Id already exists.");
            }

            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return Ok("Book added successfully!");
        }

        [HttpDelete]
        [Route("deleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookToDelete = await _context.Book.FindAsync(id);

            if (bookToDelete == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            _context.Book.Remove(bookToDelete);
            await _context.SaveChangesAsync();

            return Ok($"Book with ID {id} deleted successfully.");
        }

        [HttpPost]
        [Route("/{loanBook}")]
        public async Task<IActionResult> LoanBook(LoanRequest request)
        {
            Book book=_context.Book.FirstOrDefault(b => b.Id == request.BookId);
            if (book.Loaned)
            {
                throw new ArgumentException("Book is already loaned!");
            }

            Loan loan = _context.Loan.FirstOrDefault(l => l.UserId == request.UserId);

            User user = _context.User.FirstOrDefault(u => u.Id == request.UserId);

            if (loan != null)
            {
                throw new ArgumentException("You already loaned book!");
            }

            Loan l = new Loan
            {
                BookId = request.BookId,
                UserId = request.UserId,
                Date = DateTime.Today
            };

            _context.Loan.Add(l);
            await _context.SaveChangesAsync();
            return Ok("You loaned a book successfully!");
        }

        [HttpDelete]
        [Route("/{getBookBack}")]
        public async Task<IActionResult> GetBookBack(GetBookBackRequest request)
        {
            Loan l=_context.Loan.FirstOrDefault(l => l.BookId == request.BookId);
            if (l == null)
            {
                throw new ArgumentException("Book with that id is not loaned!");
            }
            _context.Loan.Remove(l);

            await _context.SaveChangesAsync();
            return Ok("You got book back!");
        }
    }
}
