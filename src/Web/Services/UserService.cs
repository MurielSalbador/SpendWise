using SpendWise.Core.DTOs;
using SpendWise.Core.Entities;
using SpendWise.Core.Interfaces;
using SpendWise.Web.Models.Requests;

namespace SpendWise.Core.Services
{
    // Servicio para gestionar usuarios.
    // Proporciona métodos CRUD y mapeo entre entidades y DTOs.
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;

        public UserService(IUserRepository userRepository, ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        // Obtiene todos los usuarios (solo si se desea un panel administrativo).
        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(UserDto.Create).ToList();
        }

        // Obtiene la información del usuario actual autenticado.
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            var userId = _currentUser.UserId ?? throw new Exception("Usuario no autenticado.");

            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? UserDto.Create(user) : null;
        }

        // Obtiene un usuario por ID (útil para admin o debugging).
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? UserDto.Create(user) : null;
        }

        // Crea un nuevo usuario.
        public async Task<User> AddAsync(UserRegisterRequest dto)
        {
            var existing = await _userRepository.GetUserByUsernameAsync(dto.Username);
            if (existing != null)
                throw new Exception("El nombre de usuario ya está en uso.");

            var user = new User(
                username: dto.Username,
                email: dto.Email,
                name: dto.Name,
                surname: dto.Surname,
                password: dto.Password
            );

            await _userRepository.AddAsync(user);
            return user;
        }

        // Actualiza los datos de un usuario existente.
        public async Task<bool> UpdateAsync(int id, UpdateUserRequest dto)
        {
            var currentUserId = _currentUser.UserId ?? throw new Exception("Usuario no autenticado.");

            // Solo el propio usuario o un admin podría modificarlo.
            if (currentUserId != id)
                throw new Exception("No tienes permiso para modificar este usuario.");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.UpdateProfile(dto.Username, dto.Name, dto.Surname, dto.Email);

            // Cambiar contraseña si fue proporcionada
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.ChangePassword(dto.Password);

            await _userRepository.UpdateAsync(user);
            return true;
        }

        // Elimina al usuario autenticado.
        public async Task<bool> DeleteAsync(int id)
        {
            var currentUserId = _currentUser.UserId ?? throw new Exception("Usuario no autenticado.");

            if (currentUserId != id)
                throw new Exception("No tienes permiso para eliminar este usuario.");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}
