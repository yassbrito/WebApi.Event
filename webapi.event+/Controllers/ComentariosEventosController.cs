﻿using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.event_.Domains;
using webapi.event_.Interfaces;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventosController : ControllerBase
    {
        private readonly IComentariosEventosRepository _comentariosEventosRepository;
        private readonly ContentSafetyClient _contentSafetyClient;
        public ComentariosEventosController(ContentSafetyClient contentSafetyClient ,IComentariosEventosRepository comentariosEventosRepository)
        {
            _comentariosEventosRepository = comentariosEventosRepository;
            _contentSafetyClient = contentSafetyClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ComentariosEventos comentario)
        {
            try
            {
                if (string.IsNullOrEmpty(comentario.Descricao)) 
                {
                    return BadRequest("o texto a ser moderado nao pode estar vazio!");
                }

                //criar objeto de analise do content safery
                var request = new AnalyzeTextOptions(comentario.Descricao);

                //chamar a API do content safery
                Response<AnalyzeTextResult> response = await _contentSafetyClient.AnalyzeTextAsync(request);

                //verificar se o texto analisado tem alguma severidade
                bool temConteudoImproprio = response.Value.CategoriesAnalysis.Any(c => c.Severity > 0);

                //se o comentario for improprio, nao exibe, caso ao contrario, exibe
                comentario.Exibe = !temConteudoImproprio;

                //cadastra de fato o comentario
                _comentariosEventosRepository.Cadastrar(comentario);

                return Ok();

            }
            catch (Exception e)
            {

                return BadRequest(e. Message);
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _comentariosEventosRepository.Deletar(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("ListarSomenteExibe")]
        public IActionResult GetExibe(Guid id)
        {
            try
            {
                return Ok(_comentariosEventosRepository.ListarSomenteExibe(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Get(Guid id)
        {
            try
            {
                return Ok(_comentariosEventosRepository.Listar(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("BuscarPorIdUsuario")]
        public IActionResult GetByIdUser(Guid idUsuario, Guid idEvento)
        {
            try
            {
                return Ok(_comentariosEventosRepository.BuscarPorIdUsuario(idUsuario, idEvento));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
