namespace Desafio.Cooperacode.FilaPilha.Models;

/// <summary>
/// Classe imutável que representa uma ação no histórico.
/// Imutabilidade garante que uma ação registrada não possa ser alterada.
/// </summary>
public sealed class HistoricoAcao(TipoAcao acao, string pedido)
{
    public TipoAcao Acao { get; } = acao;
    public string Pedido { get; } = pedido;
}

///// <summary>
///// Classe imutável que representa uma ação no histórico.
///// Imutabilidade garante que uma ação registrada não possa ser alterada.
///// </summary>
//public sealed class HistoricoAcao
//{
//    public TipoAcao Acao { get; }
//    public LinkedListNode<string>? Node { get; }
//    public string Pedido => Node?.Value;

//    public HistoricoAcao(TipoAcao acao, LinkedListNode<string>? node = null)
//    {
//        Acao = acao;
//        Node = node;
//    }
//}