using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using DevIO.Business.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IFornecedorService fornecedorService)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
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

        [HttpPost]
        public async Task<ActionResult<FornecedoreViewModel>> Adicionar(FornecedoreViewModel fornecedoreViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var fornecedor = fornecedoreViewModel.Adapt<Fornecedor>();

            var result = await _fornecedorService.Adicionar(fornecedor);

            if (!result) return BadRequest();

            return Ok(fornecedor);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedoreViewModel>> Atualizar(Guid id, FornecedoreViewModel fornecedoreViewModel)
        {
            if (id != fornecedoreViewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return BadRequest();

            var fornecedor = fornecedoreViewModel.Adapt<Fornecedor>();

            var result = await _fornecedorService.Atualizar(fornecedor);

            if (!result) return BadRequest();

            return Ok(fornecedor);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedoreViewModel>> Excluir(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if(fornecedor == null) return NotFound();

            var result = await _fornecedorService.Remover(id);

            if (!result) return BadRequest();

            return Ok(fornecedor);
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