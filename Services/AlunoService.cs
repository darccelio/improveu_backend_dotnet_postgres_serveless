﻿using ImproveU_backend.DatabaseConfiguration.Context;
using ImproveU_backend.Models;
using ImproveU_backend.Models.Dtos;
using ImproveU_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImproveU_backend.Services;
public class AlunoService : IAlunoService
{

    private readonly ImproveuContext _context;
    private readonly IPessoaService _pessoaService;

    public AlunoService(ImproveuContext context, IPessoaService pessoaService)
    {
        _context = context;
        _pessoaService = pessoaService;
    }

    public async Task<AlunoResponseDto> CriarAsync(AlunoCreateRequestDto alunoRequest)
    {

        if (alunoRequest is null)
            throw new ArgumentNullException(nameof(alunoRequest), "O objeto AlunoCreateRequestDto não pode ser nulo.");

        PessoaResponseDto? pessoaDto = await _pessoaService.BuscarPorCpfAsync(alunoRequest.PessoaCreateRequest.Cpf);

        if (pessoaDto is not null)
            throw new ArgumentException("Pessoa já cadastrada.");

        var usuarioPapel = await _context.Usuarios
                .Where(u => u.Id == alunoRequest.PessoaCreateRequest.UsuarioId)
                .Select(u => u.Papel)
                .FirstOrDefaultAsync();

        if (usuarioPapel != 1)
            throw new ArgumentException("O usuário vinculado não possui papel de aluno.");

        Pessoa pessoa = new Pessoa(alunoRequest.PessoaCreateRequest.Cpf,
                alunoRequest.PessoaCreateRequest.Nome,
                alunoRequest.PessoaCreateRequest.UsuarioId);

        _context.Pessoas.Add(pessoa);

        Aluno novoAluno = new Aluno() { Pessoa = pessoa, PessoaId = pessoa.Id };

        await _context.Alunos.AddAsync(novoAluno);
        await _context.SaveChangesAsync();
        AlunoResponseDto alunoRespDto = new AlunoResponseDto(novoAluno);
        return alunoRespDto;
    }

    public async Task<IEnumerable<AlunoResponseDto>> BuscarAsync(int skip, int take)
    {
        List<Aluno> alunos = await _context.Alunos.Include(aluno => aluno.Pessoa).AsNoTracking().Skip(skip).Take(take).ToListAsync();

        return alunos.Select(aluno => new AlunoResponseDto(aluno));
    }

    public async Task<AlunoResponseDto> BuscarPorIdAsync(int id)
    {
        Aluno? aluno = await _context.Alunos.FirstOrDefaultAsync(al => al.Id == id);

        if (aluno is null)
            return null;

        return new AlunoResponseDto(aluno);
    }

    public async Task AtualizarAsync(int id, AlunoUpdateRequestDto alunoRequest)
    {
        //if (alunoRequest is null)
        //    throw new ArgumentNullException(nameof(alunoRequest), "O objeto EdFisicoRequestDto não pode ser nulo.");

        //if(id != alunoRequest.Id)
        //    throw new ArgumentException("Id do objeto diferente do id da rota.");

        //Aluno? aluno = await _context.Alunos.Include(aluno => aluno.Pessoa).FirstOrDefaultAsync(al => al.Id == id);

        //if (aluno is null)
        //    throw new ArgumentException("Aluno não encontrado.");

        //aluno.Pessoa.Nome = alunoRequest?.Nome;
        //aluno.Pessoa.Cpf = alunoRequest?.Cpf;

        //_context.Alunos.Update(aluno);
        //_context.Entry(aluno.Pessoa).State = EntityState.Modified;

        //return _context?.SaveChangesAsync();

        throw new NotImplementedException();

    }

    public Task InativarPorIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
