using System.Collections.Generic;

public class StringAndBoolRet {
    /// <summary>
    ///     The string value.
    /// </summary>
    public List<string> StringList { get; private set; }

    /// <summary>
    ///     The boolean value.
    /// </summary>
    public bool BoolValue { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="StringAndBoolRet" /> class.
    /// </summary>
    public StringAndBoolRet(List<string> stringList, bool boolValue) {
        StringList = stringList;
        BoolValue = boolValue;
    }
}