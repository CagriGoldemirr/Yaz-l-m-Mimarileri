using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using StockMgmt.Core.DTOs.User;
using StockMgmt.Core.Entities;
using StockMgmt.Data;
using StockMgmt.Service.Interfaces;

namespace StockMgmt.Service.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> CreateAsync(UserCreateDto createDto)
    {
        // Check Username uniqueness
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == createDto.Username && !u.IsDeleted);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{createDto.Username}' is already taken.");
        }

        var user = _mapper.Map<User>(createDto);
        
        // Hash password
        user.PasswordHash = HashPassword(createDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _context.Entry(user)
            .Collection(u => u.Reviews)
            .LoadAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> UpdateAsync(int id, UserUpdateDto updateDto)
    {
        var user = await _context.Users
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        // Check Username uniqueness if updated
        if (!string.IsNullOrWhiteSpace(updateDto.Username) && updateDto.Username != user.Username)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == updateDto.Username && u.Id != id && !u.IsDeleted);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Username '{updateDto.Username}' is already taken.");
            }
            user.Username = updateDto.Username;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Password))
        {
            user.PasswordHash = HashPassword(updateDto.Password);
        }

        if (updateDto.Role.HasValue)
            user.Role = updateDto.Role.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        return user == null ? null : _mapper.Map<UserResponseDto>(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.Reviews)
            .Where(u => !u.IsDeleted)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
        {
            return false;
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

