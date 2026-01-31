using Desafio.Cooperacode.FilaPilha.Models;

namespace Desafio.Cooperacode.FilaPilha.Services;

/// <summary>
/// Desafio: utilizando estruturas de dados adequadas.
/// - Queue<T> para fila de pedidos (FIFO)
/// - Stack<T> para histórico de ações (LIFO)
/// </summary>
public sealed class SistemaAtendimento
{
    private Queue<string> filaPedidos = new();
    private readonly Stack<HistoricoAcao> historicoAcoes = new();

    /// <summary>
    /// Adiciona um novo pedido à fila de atendimento (FIFO).
    /// Operação O(1).
    /// </summary>
    public void AdicionarPedido(string pedido)
    {
        if (string.IsNullOrWhiteSpace(pedido))
            throw new ArgumentException("Pedido não pode ser vazio", nameof(pedido));

        filaPedidos.Enqueue(pedido);
        historicoAcoes.Push(new HistoricoAcao(TipoAcao.Adicionar, pedido));
    }

    /// <summary>
    /// Atende o próximo pedido da fila (FIFO - First In First Out)
    /// </summary>
    public string? AtenderProximo()
    {
        if (filaPedidos.Count == 0)
            return null;

        string pedido = filaPedidos.Dequeue();
        historicoAcoes.Push(new HistoricoAcao(TipoAcao.Atender, pedido));
        return pedido;
    }

    /// <summary>
    /// Desfaz a última ação realizada (LIFO - Last In First Out)
    /// </summary>
    public HistoricoAcao? DesfazerUltimaAcao()
    {
        if (historicoAcoes.Count == 0)
            return null;

        var ultimaAcao = historicoAcoes.Pop();

        // Reverte a ação
        if (ultimaAcao.Acao == TipoAcao.Adicionar)
        {
            // Remove o pedido específico da fila (recriando a fila sem ele), ou seja,
            // remove o último pedido adicionado (otimização: mantemos uma referência interna)
            // Nota: Em cenários reais, poderíamos usar LinkedList para remoção eficiente do final
            // mas para este desafio, mantemos a simplicidade com Queue + lógica de reversão

            var pedidosTemp = new Queue<string>();
            bool removido = false;

            while (filaPedidos.Count > 0)
            {
                var p = filaPedidos.Dequeue();
                if (p == ultimaAcao.Pedido && !removido)
                {
                    removido = true; // Remove apenas a primeira ocorrência
                    continue;
                }
                pedidosTemp.Enqueue(p);
            }

            // Restaura a fila sem o pedido removido
            while (pedidosTemp.Count > 0)
            {
                filaPedidos.Enqueue(pedidosTemp.Dequeue());
            }
        }
        else if (ultimaAcao.Acao == TipoAcao.Atender)
        {
            // Recoloca o pedido no início da fila (comportamento FIFO preservado)
            var pedidosTemp = new Queue<string>();
            pedidosTemp.Enqueue(ultimaAcao.Pedido);

            while (filaPedidos.Count > 0)
                pedidosTemp.Enqueue(filaPedidos.Dequeue());

            filaPedidos = pedidosTemp;
        }

        return ultimaAcao;
    }

    /// <summary>
    /// Retorna a quantidade de pedidos aguardando atendimento
    /// </summary>
    public int ObterQuantidadePedidos() => filaPedidos.Count;

    /// <summary>
    /// Retorna a quantidade de ações no histórico
    /// </summary>
    public int ObterQuantidadeHistorico() => historicoAcoes.Count;

    /// <summary>
    /// Visualiza o próximo pedido sem removê-lo da fila
    /// </summary>
    public string? VisualizarProximo()
    {
        return filaPedidos.Count > 0 ? filaPedidos.Peek() : null;
    }
}

//public sealed class SistemaAtendimento
//{
//    private readonly LinkedList<string> filaPedidos = new LinkedList<string>();

//    private readonly Stack<HistoricoAcao> historicoAcoes = new Stack<HistoricoAcao>();

//    /// <summary>
//    /// Adiciona um novo pedido à fila de atendimento (FIFO).
//    /// Complexidade da operação: O(1).
//    /// </summary>
//    public void AdicionarPedido(string pedido)
//    {
//        if (string.IsNullOrWhiteSpace(pedido))
//            throw new ArgumentException("Pedido não pode ser vazio", nameof(pedido));

//        var node = filaPedidos.AddLast(pedido);
//        historicoAcoes.Push(new HistoricoAcao(TipoAcao.Adicionar, node));

//        Console.WriteLine($"[INFO] Pedido adicionado: {pedido}");
//    }

//    /// <summary>
//    /// Atende o próximo pedido da fila (FIFO - First In First Out)
//    /// </summary>
//    public string? AtenderProximo()
//    {
//        if (filaPedidos.Count == 0)
//            return null;

//        var primeiroNo = filaPedidos.First!;

//        filaPedidos.RemoveFirst();
//        historicoAcoes.Push(new HistoricoAcao(TipoAcao.Atender, primeiroNo));

//        Console.WriteLine($"[INFO] Pedido atendido: {primeiroNo.Value}");

//        return primeiroNo.Value;
//    }

//    /// <summary>
//    /// Desfaz a última ação realizada (LIFO - Last In First Out)
//    /// Complexidade: O(1) para ambos os casos
//    /// </summary>
//    public HistoricoAcao? DesfazerUltimaAcao()
//    {
//        if (historicoAcoes.Count == 0)
//            return null;

//        var ultimaAcao = historicoAcoes.Pop(); // LIFO explícito
//        Console.WriteLine($"[INFO] Desfazendo: {ultimaAcao.Acao} '{ultimaAcao.Pedido}'");

//        if (ultimaAcao.Acao == TipoAcao.Adicionar)
//        {
//            // Remove o nó específico que foi adicionado
//            // O(1) porque temos a referência direta ao nó
//            // ou seja, temos uma remoção direta via referência
//            filaPedidos.Remove(ultimaAcao.Node!);
//        }
//        else if (ultimaAcao.Acao == TipoAcao.Atender)
//        {
//            // Recoloca o nó no início da fila
//            // O(1) porque temos a referência
//            filaPedidos.AddFirst(ultimaAcao.Node!);
//        }

//        return ultimaAcao;
//    }

//    /// <summary>
//    /// Retorna a quantidade de pedidos aguardando atendimento
//    /// </summary>
//    public int ObterQuantidadePedidos()
//        => filaPedidos.Count;

//    /// <summary>
//    /// Retorna a quantidade de ações no histórico
//    /// </summary>
//    public int ObterQuantidadeHistorico()
//        => historicoAcoes.Count;

//    /// <summary>
//    /// Visualiza o próximo pedido sem removê-lo da fila
//    /// </summary>
//    public string? VisualizarProximo()
//        => filaPedidos.Count > 0 ? filaPedidos.First!.Value : null;

//    /// <summary>
//    /// Visualiza o último pedido
//    /// </summary>
//    public string? VisualizarUltimo()
//        => filaPedidos.Count > 0 ? filaPedidos.Last!.Value : null;
//}