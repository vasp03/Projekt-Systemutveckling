namespace Goodot15.Scripts.Game.Model;

/// <summary>
///     A simple construct, holds 2 values being <see cref="L" /> and <see cref="R" />.<br />
///     Values are accessed through <see cref="Left" /> and <see cref="Right" /> for <see cref="L" /> and <see cref="R" />
///     respectively.
/// </summary>
/// <typeparam name="L">Left Datatype</typeparam>
/// <typeparam name="R">Right Datatype</typeparam>
public record Pair<L, R> {
    /// <summary>
    ///     Constructs a Pair object. Left and Right being populated with default values
    /// </summary>
    public Pair() {
        Left = default;
        Right = default;
    }

    /// <summary>
    ///     Constructs a Pair object. Left and Right being populated by the supplied parameters
    /// </summary>
    /// <param name="left">Left value</param>
    /// <param name="right">Right value</param>
    public Pair(L left, R right) {
        Left = left;
        Right = right;
    }

    /// <summary>
    ///     Left value of this <see cref="Pair{L,R}" /> object
    /// </summary>
    public L Left { get; set; }

    /// <summary>
    ///     Right value of this <see cref="Pair{L,R}" /> object
    /// </summary>
    public R Right { get; set; }

    public virtual bool Equals(Pair<L, R> other) {
        return Left.Equals(other is null
            ? default
            : other.Left) && Right.Equals(other is null
            ? default
            : other.Right);
    }

    public override string ToString() {
        return $"Left (T): {Left}, Right (K): {Right}";
    }
}