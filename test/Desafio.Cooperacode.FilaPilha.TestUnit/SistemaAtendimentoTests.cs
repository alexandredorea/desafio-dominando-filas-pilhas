using Desafio.Cooperacode.FilaPilha.Models;
using Desafio.Cooperacode.FilaPilha.Services;

namespace Desafio.Cooperacode.FilaPilha.TestUnit;

public sealed class SistemaAtendimentoTests
{
    #region Testes de AdicionarPedido

    [Fact]
    public void AdicionarPedido_PedidoValido_DeveAdicionarNaFila()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Hambúrguer");

        // Assert
        Assert.Equal(1, sistema.ObterQuantidadePedidos());
        Assert.Equal(1, sistema.ObterQuantidadeHistorico());
        Assert.Equal("Hambúrguer", sistema.VisualizarProximo());
    }

    [Fact]
    public void AdicionarPedido_MultiplosPedidos_DeveManterOrdemFIFO()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");
        sistema.AdicionarPedido("Pedido 3");

        // Assert
        Assert.Equal(3, sistema.ObterQuantidadePedidos());
        Assert.Equal("Pedido 1", sistema.VisualizarProximo());
    }

    [Fact]
    public void AdicionarPedido_PedidoNull_DeveLancarArgumentException()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => sistema.AdicionarPedido(null!));
        Assert.Equal("pedido", exception.ParamName);
    }

    [Fact]
    public void AdicionarPedido_PedidoVazio_DeveLancarArgumentException()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => sistema.AdicionarPedido(string.Empty));
        Assert.Equal("pedido", exception.ParamName);
    }

    [Fact]
    public void AdicionarPedido_PedidoApenasEspacos_DeveLancarArgumentException()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => sistema.AdicionarPedido(new string(' ', 5)));
        Assert.Equal("pedido", exception.ParamName);
    }

    [Fact]
    public void AdicionarPedido_PedidoDuplicado_DevePermitirAdicao()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Pizza");
        sistema.AdicionarPedido("Pizza");

        // Assert
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    [Fact]
    public void AdicionarPedido_DeveRegistrarNoHistoricoComoAdicionar()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Refrigerante");
        var historico = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.NotNull(historico);
        Assert.Equal(TipoAcao.Adicionar, historico.Acao);
        Assert.Equal("Refrigerante", historico.Pedido);
    }

    #endregion Testes de AdicionarPedido

    #region Testes de AtenderProximo

    [Fact]
    public void AtenderProximo_FilaComPedidos_DeveRetornarPrimeiroFIFO()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");
        sistema.AdicionarPedido("Pedido 3");

        // Act
        var primeiro = sistema.AtenderProximo();

        // Assert
        Assert.Equal("Pedido 1", primeiro);
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    [Fact]
    public void AtenderProximo_FilaVazia_DeveRetornarNull()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        var resultado = sistema.AtenderProximo();

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void AtenderProximo_AtenderTodosPedidos_DeveEsvaziarFila()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");

        // Act
        sistema.AtenderProximo();
        sistema.AtenderProximo();

        // Assert
        Assert.Equal(0, sistema.ObterQuantidadePedidos());
        Assert.Null(sistema.AtenderProximo());
    }

    [Fact]
    public void AtenderProximo_DeveManterOrdemFIFO()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("A");
        sistema.AdicionarPedido("B");
        sistema.AdicionarPedido("C");

        // Act
        var primeiro = sistema.AtenderProximo();
        var segundo = sistema.AtenderProximo();
        var terceiro = sistema.AtenderProximo();

        // Assert
        Assert.Equal("A", primeiro);
        Assert.Equal("B", segundo);
        Assert.Equal("C", terceiro);
    }

    [Fact]
    public void AtenderProximo_DeveRegistrarNoHistoricoComoAtender()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Batata Frita");

        // Act
        sistema.AtenderProximo();
        var historico = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.NotNull(historico);
        Assert.Equal(TipoAcao.Atender, historico.Acao);
        Assert.Equal("Batata Frita", historico.Pedido);
    }

    [Fact]
    public void AtenderProximo_AposAtender_HistoricoDeveAumentar()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");

        // Act
        sistema.AtenderProximo();

        // Assert
        Assert.Equal(2, sistema.ObterQuantidadeHistorico()); // 1 adicionar + 1 atender
    }

    #endregion Testes de AtenderProximo

    #region Testes de DesfazerUltimaAcao

    [Fact]
    public void DesfazerUltimaAcao_HistoricoVazio_DeveRetornarNull()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        var resultado = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void DesfazerUltimaAcao_DesfazerAdicionar_DeveRemoverDaFila()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");
        sistema.AdicionarPedido("Pedido 3");

        // Act
        var acaoDesfeita = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.NotNull(acaoDesfeita);
        Assert.Equal(TipoAcao.Adicionar, acaoDesfeita.Acao);
        Assert.Equal("Pedido 3", acaoDesfeita.Pedido);
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    [Fact]
    public void DesfazerUltimaAcao_DesfazerAtender_DeveRecolocarNoInicio()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");
        sistema.AtenderProximo();

        // Act
        var acaoDesfeita = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.NotNull(acaoDesfeita);
        Assert.Equal(TipoAcao.Atender, acaoDesfeita.Acao);
        Assert.Equal("Pedido 1", acaoDesfeita.Pedido);
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
        Assert.Equal("Pedido 1", sistema.VisualizarProximo());
    }

    [Fact]
    public void DesfazerUltimaAcao_SegueOrdemLIFO()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("A");
        sistema.AdicionarPedido("B");
        sistema.AtenderProximo();

        // Act
        var primeira = sistema.DesfazerUltimaAcao(); // Desfaz atender
        var segunda = sistema.DesfazerUltimaAcao();  // Desfaz adicionar B
        var terceira = sistema.DesfazerUltimaAcao(); // Desfaz adicionar A

        // Assert
        Assert.Equal(TipoAcao.Atender, primeira?.Acao);
        Assert.Equal(TipoAcao.Adicionar, segunda?.Acao);
        Assert.Equal("B", segunda?.Pedido);
        Assert.Equal(TipoAcao.Adicionar, terceira?.Acao);
        Assert.Equal("A", terceira?.Pedido);
    }

    [Fact]
    public void DesfazerUltimaAcao_DesfazerTodasAcoes_DeveEsvaziarHistorico()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");

        // Act
        sistema.DesfazerUltimaAcao();
        sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Equal(0, sistema.ObterQuantidadeHistorico());
        Assert.Null(sistema.DesfazerUltimaAcao());
    }

    [Fact]
    public void DesfazerUltimaAcao_DesfazerAdicionarComPedidoDuplicado_DeveRemoverApenasUm()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pizza");
        sistema.AdicionarPedido("Pizza");
        sistema.AdicionarPedido("Pizza");

        // Act
        sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    [Fact]
    public void DesfazerUltimaAcao_DesfazerAposFilaVazia_DeveRecolocarPedido()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Único Pedido");
        sistema.AtenderProximo(); // Fila fica vazia

        // Act
        sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Equal(1, sistema.ObterQuantidadePedidos());
        Assert.Equal("Único Pedido", sistema.VisualizarProximo());
    }

    #endregion Testes de DesfazerUltimaAcao

    #region Testes de VisualizarProximo

    [Fact]
    public void VisualizarProximo_FilaComPedidos_DeveRetornarPrimeiroSemRemover()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido 1");
        sistema.AdicionarPedido("Pedido 2");

        // Act
        var visualizado = sistema.VisualizarProximo();

        // Assert
        Assert.Equal("Pedido 1", visualizado);
        Assert.Equal(2, sistema.ObterQuantidadePedidos()); // Não removeu
    }

    [Fact]
    public void VisualizarProximo_FilaVazia_DeveRetornarNull()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        var resultado = sistema.VisualizarProximo();

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void VisualizarProximo_ChamadasMultiplas_DeveRetornarMesmoPedido()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Pedido Fixo");

        // Act
        var primeira = sistema.VisualizarProximo();
        var segunda = sistema.VisualizarProximo();
        var terceira = sistema.VisualizarProximo();

        // Assert
        Assert.Equal("Pedido Fixo", primeira);
        Assert.Equal("Pedido Fixo", segunda);
        Assert.Equal("Pedido Fixo", terceira);
        Assert.Equal(1, sistema.ObterQuantidadePedidos());
    }

    #endregion Testes de VisualizarProximo

    #region Testes de ObterQuantidadePedidos

    [Fact]
    public void ObterQuantidadePedidos_FilaVazia_DeveRetornarZero()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        var quantidade = sistema.ObterQuantidadePedidos();

        // Assert
        Assert.Equal(0, quantidade);
    }

    [Fact]
    public void ObterQuantidadePedidos_DeveRefletirEstadoAtual()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act & Assert
        Assert.Equal(0, sistema.ObterQuantidadePedidos());

        sistema.AdicionarPedido("P1");
        Assert.Equal(1, sistema.ObterQuantidadePedidos());

        sistema.AdicionarPedido("P2");
        Assert.Equal(2, sistema.ObterQuantidadePedidos());

        sistema.AtenderProximo();
        Assert.Equal(1, sistema.ObterQuantidadePedidos());

        sistema.DesfazerUltimaAcao();
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    #endregion Testes de ObterQuantidadePedidos

    #region Testes de ObterQuantidadeHistorico

    [Fact]
    public void ObterQuantidadeHistorico_Inicial_DeveRetornarZero()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        var quantidade = sistema.ObterQuantidadeHistorico();

        // Assert
        Assert.Equal(0, quantidade);
    }

    [Fact]
    public void ObterQuantidadeHistorico_DeveAumentarComAcoes()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act & Assert
        sistema.AdicionarPedido("P1");
        Assert.Equal(1, sistema.ObterQuantidadeHistorico());

        sistema.AdicionarPedido("P2");
        Assert.Equal(2, sistema.ObterQuantidadeHistorico());

        sistema.AtenderProximo();
        Assert.Equal(3, sistema.ObterQuantidadeHistorico());
    }

    [Fact]
    public void ObterQuantidadeHistorico_DeveDiminuirAoDesfazer()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("P1");
        sistema.AdicionarPedido("P2");

        // Act
        sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Equal(1, sistema.ObterQuantidadeHistorico());
    }

    #endregion Testes de ObterQuantidadeHistorico

    #region Testes de Integração e Cenários Complexos

    [Fact]
    public void FluxoCompleto_AdicionarAtenderDesfazer_DeveManterConsistencia()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Pedido A");
        sistema.AdicionarPedido("Pedido B");
        sistema.AdicionarPedido("Pedido C");
        sistema.AtenderProximo(); // Atende A
        sistema.AtenderProximo(); // Atende B
        sistema.DesfazerUltimaAcao(); // Desfaz atendimento de B
        sistema.DesfazerUltimaAcao(); // Desfaz atendimento de A

        // Assert
        Assert.Equal(3, sistema.ObterQuantidadePedidos());
        Assert.Equal("Pedido A", sistema.VisualizarProximo());
    }

    [Fact]
    public void CenarioReal_Lanchonete_FluxoDiario()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Manhã: chegam 3 clientes
        sistema.AdicionarPedido("Café com Leite");
        sistema.AdicionarPedido("Pão na Chapa");
        sistema.AdicionarPedido("Suco de Laranja");

        // Atende os 2 primeiros
        var atendido1 = sistema.AtenderProximo();
        var atendido2 = sistema.AtenderProximo();

        // Cliente adiciona pedido errado, desfaz
        sistema.AdicionarPedido("Refrigerante");
        sistema.DesfazerUltimaAcao();

        // Novo cliente
        sistema.AdicionarPedido("Hambúrguer");

        // Assert
        Assert.Equal("Café com Leite", atendido1);
        Assert.Equal("Pão na Chapa", atendido2);
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
        Assert.Equal("Suco de Laranja", sistema.VisualizarProximo());
    }

    [Fact]
    public void DesfazerMultiplasVezes_DeveReverterCorreatamente()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("A");
        sistema.AdicionarPedido("B");
        sistema.AdicionarPedido("C");
        sistema.AtenderProximo();
        sistema.AtenderProximo();

        // Act - Desfazer tudo
        sistema.DesfazerUltimaAcao(); // Desfaz atender B
        sistema.DesfazerUltimaAcao(); // Desfaz atender A
        sistema.DesfazerUltimaAcao(); // Desfaz adicionar C
        sistema.DesfazerUltimaAcao(); // Desfaz adicionar B
        sistema.DesfazerUltimaAcao(); // Desfaz adicionar A

        // Assert
        Assert.Equal(0, sistema.ObterQuantidadePedidos());
        Assert.Equal(0, sistema.ObterQuantidadeHistorico());
    }

    [Fact]
    public void AdicionarAtenderIntercalado_DeveManterOrdem()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("1");
        sistema.AdicionarPedido("2");
        var p1 = sistema.AtenderProximo();
        sistema.AdicionarPedido("3");
        sistema.AdicionarPedido("4");
        var p2 = sistema.AtenderProximo();
        var p3 = sistema.AtenderProximo();

        // Assert
        Assert.Equal("1", p1);
        Assert.Equal("2", p2);
        Assert.Equal("3", p3);
        Assert.Equal(1, sistema.ObterQuantidadePedidos());
        Assert.Equal("4", sistema.VisualizarProximo());
    }

    [Fact]
    public void DesfazerAdicionar_NaMesmaPosicao_DeveRemoverCorreto()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Primeiro");
        sistema.AdicionarPedido("Segundo");
        sistema.AdicionarPedido("Terceiro");

        // Act - Remove o último adicionado
        sistema.DesfazerUltimaAcao();

        // Assert
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
        Assert.Equal("Primeiro", sistema.VisualizarProximo());
    }

    [Fact]
    public void HistoricoAcao_DeveSerImutavel()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        sistema.AdicionarPedido("Teste");

        // Act
        var acao = sistema.DesfazerUltimaAcao();

        // Assert
        Assert.NotNull(acao);
        Assert.Equal(TipoAcao.Adicionar, acao.Acao);
        Assert.Equal("Teste", acao.Pedido);
    }

    #endregion Testes de Integração e Cenários Complexos

    #region Testes de Performance e Edge Cases

    [Fact]
    public void GrandeVolumeDePedidos_DeveManterPerformance()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        const int quantidade = 1000;

        // Act
        for (int i = 0; i < quantidade; i++)
        {
            sistema.AdicionarPedido($"Pedido {i}");
        }

        // Assert
        Assert.Equal(quantidade, sistema.ObterQuantidadePedidos());
        Assert.Equal("Pedido 0", sistema.VisualizarProximo());
    }

    [Fact]
    public void AtenderGrandeVolume_DeveManterOrdemFIFO()
    {
        // Arrange
        var sistema = new SistemaAtendimento();
        for (int i = 0; i < 100; i++)
        {
            sistema.AdicionarPedido($"P{i}");
        }

        // Act & Assert
        for (int i = 0; i < 100; i++)
        {
            var atendido = sistema.AtenderProximo();
            Assert.Equal($"P{i}", atendido);
        }

        Assert.Equal(0, sistema.ObterQuantidadePedidos());
    }

    [Fact]
    public void PedidoComCaracteresEspeciais_DeveSerAceito()
    {
        // Arrange
        var sistema = new SistemaAtendimento();

        // Act
        sistema.AdicionarPedido("Café com açúcar (2x)");
        sistema.AdicionarPedido("X-Burguer @#$%");

        // Assert
        Assert.Equal(2, sistema.ObterQuantidadePedidos());
    }

    #endregion Testes de Performance e Edge Cases
}