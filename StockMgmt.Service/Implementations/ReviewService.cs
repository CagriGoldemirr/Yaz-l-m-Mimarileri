using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockMgmt.Core.DTOs.Review;
using StockMgmt.Core.Entities;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReviewResponseDto> CreateAsync(ReviewCreateDto createDto)
    {
        // Verify User exists
        var user = await _context.Users.FindAsync(createDto.UserId);
        if (user == null || user.IsDeleted)
        {
            throw new KeyNotFoundException($"User with ID {createDto.UserId} not found.");
        }

        // Verify Product exists
        var product = await _context.Products.FindAsync(createDto.ProductId);
        if (product == null || product.IsDeleted)
        {
            throw new KeyNotFoundException($"Product with ID {createDto.ProductId} not found.");
        }

        // Validate rating
        if (createDto.Rating < 1 || createDto.Rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.");
        }

        var review = _mapper.Map<Review>(createDto);
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Load related entities for response
        await _context.Entry(review)
            .Reference(r => r.User)
            .LoadAsync();
        await _context.Entry(review)
            .Reference(r => r.Product)
            .LoadAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<ReviewResponseDto> UpdateAsync(int id, ReviewUpdateDto updateDto)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {id} not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Title))
            review.Title = updateDto.Title;
        if (!string.IsNullOrWhiteSpace(updateDto.Content))
            review.Content = updateDto.Content;
        
        if (updateDto.Rating.HasValue)
        {
            if (updateDto.Rating.Value < 1 || updateDto.Rating.Value > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }
            review.Rating = updateDto.Rating.Value;
        }

        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<ReviewResponseDto?> GetByIdAsync(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return review == null ? null : _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetAllAsync()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetByProductIdAsync(int productId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId && !r.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (review == null)
        {
            return false;
        }

        review.IsDeleted = true;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
