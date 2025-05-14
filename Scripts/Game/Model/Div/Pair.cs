namespace Goodot15.Scripts.Game.Model;

/// <summary>
///     A simple construct, holds 2 values being <see cref="T" /> and <see cref="K" />.<br />
///     Values are accessed through <see cref="Left" /> and <see cref="Right" /> for <see cref="T" /> and <see cref="K" />
///     respectively.
/// </summary>
/// <param name="Left"></param>
/// <param name="Right"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="K"></typeparam>
public record Pair<T, K>(T Left, K Right) {
    public T Left { get; set; } = Left;
    public K Right { get; set; } = Right;

    public virtual bool Equals(Pair<T, K> other) {
        return Left.Equals(other is null ? default : other.Left) && Right.Equals(other is null ? default : other.Right);
    }

    public override string ToString() {
        return $"Left (T): {Left}, Right (K): {Right}";
    }
}