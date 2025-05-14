namespace Goodot15.Scripts.Game.Model;

/// <summary>
///     A simple construct, holds 2 values being <see cref="L" /> and <see cref="R" />.<br />
///     Values are accessed through <see cref="Left" /> and <see cref="Right" /> for <see cref="L" /> and <see cref="R" />
///     respectively.
/// </summary>
/// <param name="Left"></param>
/// <param name="Right"></param>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public record Pair<L, R> {
    public Pair() {
        Left = default;
        Right = default;
    }

    public Pair(L left, R right) {
        Left = left;
        Right = right;
    }

    public L Left { get; set; }
    public R Right { get; set; }

    public virtual bool Equals(Pair<L, R> other) {
        return Left.Equals(other is null ? default : other.Left) && Right.Equals(other is null ? default : other.Right);
    }

    public override string ToString() {
        return $"Left (T): {Left}, Right (K): {Right}";
    }
}