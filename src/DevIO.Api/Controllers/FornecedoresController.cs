using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IFornecedorService fornecedorService,
                                      IEnderecoRepository enderecoRepository,
                                      INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedoreViewModel>> ObterTodos()
        {
            var fornecedor = await _fornecedorRepository.ObterTodos();

            return fornecedor.Adapt<IEnumerable<FornecedoreViewModel>>();
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<FornecedoreViewModel>> ObterPorId(Guid Id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(Id);

            if (fornecedor == null)
                return NotFound();

            return Ok(fornecedor);
        }

        [HttpGet("obter-endereco/{id:Guid}")]
        public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
        {
            return CustomResponse(_enderecoRepository.ObterPorId(id).Adapt<EnderecoViewModel>());
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedoreViewModel>> Adicionar(FornecedoreViewModel fornecedoreViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(fornecedoreViewModel.Adapt<Fornecedor>());

            return CustomResponse(fornecedoreViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedoreViewModel>> Atualizar(Guid id, FornecedoreViewModel fornecedoreViewModel)
        {
            if (id != fornecedoreViewModel.Id)
            {
                NotificarErro("Id informado não é o mesmo na query");
                return CustomResponse(fornecedoreViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(fornecedoreViewModel);

            var result = await _fornecedorService.Atualizar(fornecedoreViewModel.Adapt<Fornecedor>());

            if (!result) return BadRequest();

            return CustomResponse(fornecedoreViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("Id informado não é o mesmo na query");
                return CustomResponse(enderecoViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(enderecoViewModel);

            await _enderecoRepository.Atualizar(enderecoViewModel.Adapt<Endereco>());

            return CustomResponse(enderecoViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedoreViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse(fornecedorViewModel);
        }

        private async Task<FornecedoreViewModel> ObterFornecedorProdutosEndereco(Guid Id)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorProdutosEndereco(Id);

            return fornecedor.Adapt<FornecedoreViewModel>();
        }

        private async Task<FornecedoreViewModel> ObterFornecedorEndereco(Guid Id)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorEndereco(Id);

            return fornecedor.Adapt<FornecedoreViewModel>();
        }
    }
}