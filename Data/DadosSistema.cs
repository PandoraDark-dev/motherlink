
using Materna.API.Models;

namespace Materna.API.Data;

public static class DadosSistema
{
    public static List<Gestante> Gestantes { get; set; } = new()
    {
        new Gestante
        {
            Id = 1,
            Nome = "Maria Silva",
            CPF = "12345678900",
            CartaoSUS = "898001234567890",
            Senha = "maria123",
            DUM = new DateTime(2024, 5, 11),
            DPP = new DateTime(2025, 2, 15)
        }
    };

    public static List<Profissional> Profissionais { get; set; } = new()
    {
        new Profissional
        {
            Id = 1,
            Nome = "Dra. Ana Souza",
            CPF = "98765432100",
            Registro = "CRM12345",
            Especialidade = "Obstetrícia",
            Email = "ana@materna.com",
            Senha = "ana123",
            Funcao = "Profissional"
        },
        new Profissional
        {
            Id = 2,
            Nome = "Maria Eduarda",
            CPF = "2345679012",
            Registro = "123456",
            Especialidade = "Enfermeira-chefe",
            Email = "eduarda@motherlink.com",
            Senha = "duda123",
            Funcao = "EnfermeiraChefe"
        }
    };

    public static List<Consulta> Consultas { get; set; } = new()
    {
        new Consulta
        {
            Id = 1,
            GestanteId = 1,
            ProfissionalId = 1,
            Data = new DateTime(2025, 1, 15, 14, 0, 0),
            Tipo = "Pré-natal",
            Observacoes = "Consulta de acompanhamento"
        }
    };

    public static List<Exame> Exames { get; set; } = new()
    {
        new Exame
        {
            Id = 1,
            GestanteId = 1,
            Nome = "Ultrassonografia",
            Data = new DateTime(2025, 1, 10),
            Resultado = "Normal"
        }
    };

    public static List<PlanoParto> PlanosParto { get; set; } = new();

    public static List<Suplemento> Suplementos { get; set; } = new();

    public static List<RegistroSuplemento> RegistrosSuplementos { get; set; } = new();

    public static List<Mensagem> Mensagens { get; set; } = new();
}
