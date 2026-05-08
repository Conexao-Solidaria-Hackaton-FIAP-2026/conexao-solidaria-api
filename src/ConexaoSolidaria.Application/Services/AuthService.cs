using ConexaoSolidaria.Application.Abstractions;
using ConexaoSolidaria.Application.DTOs;
using ConexaoSolidaria.Domain.Entities;
using ConexaoSolidaria.Domain.Enums;
using ConexaoSolidaria.Domain.Exceptions;

namespace ConexaoSolidaria.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUsuarioRepository usuarios, IUnitOfWork uow,
                       IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _usuarios = usuarios;
        _uow = uow;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Guid> CadastrarDoadorAsync(CadastrarDoadorRequest req, CancellationToken ct)
    {
        if (await _usuarios.EmailExisteAsync(req.Email, ct))
            throw new DomainException("Email ja cadastrado.");

        var usuario = new Usuario(req.NomeCompleto, req.Email, req.Cpf,
                                   _hasher.Hash(req.Senha), UsuarioRole.Doador);
        await _usuarios.AdicionarAsync(usuario, ct);
        await _uow.SalvarAsync(ct);
        return usuario.Id;
    }

    public async Task<Guid> CadastrarGestorAsync(CadastrarGestorRequest req, CancellationToken ct)
    {
        if (await _usuarios.EmailExisteAsync(req.Email, ct))
            throw new DomainException("Email ja cadastrado.");

        var usuario = new Usuario(req.NomeCompleto, req.Email, req.Cpf,
                                   _hasher.Hash(req.Senha), UsuarioRole.GestorONG);
        await _usuarios.AdicionarAsync(usuario, ct);
        await _uow.SalvarAsync(ct);
        return usuario.Id;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var usuario = await _usuarios.ObterPorEmailAsync(req.Email, ct)
                      ?? throw new DomainException("Credenciais invalidas.");
        if (!_hasher.Verify(req.Senha, usuario.SenhaHash))
            throw new DomainException("Credenciais invalidas.");

        var token = _jwt.GerarToken(usuario);
        return new LoginResponse(token, usuario.Role.ToString(), usuario.NomeCompleto);
    }
}
