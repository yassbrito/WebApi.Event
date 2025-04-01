﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.event_.Domains
{
    [Table("TiposUsuarios")]
    public class TiposUsuarios
    {
        [Key]
        public Guid IdTipoUsuario { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        [Required(ErrorMessage = "Tipo do usuário obrigatório!")]
        public string? TituloTipoUsuario { get; set; }
    }
}
