using System.Collections.Generic;

public class StringAndBoolRet {
    /// <summary>
    ///     Initializes a new instance of the <see cref="StringAndBoolRet" /> class.
    /// </summary>
    public StringAndBoolRet(List<string> stringsValue, bool boolValue) {
        StringsValue = stringsValue;
        BoolValue = boolValue;
    }

    /// <summary>
    ///     The string value.
    /// </summary>
    public IReadOnlyCollection<string> StringsValue { get; private set; }

    /// <summary>
    ///     The boolean value.
    /// </summary>
    public bool BoolValue { get; set; }
}