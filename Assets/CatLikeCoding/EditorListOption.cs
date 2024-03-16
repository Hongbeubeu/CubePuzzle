using System;

[Flags]
public enum EditorListOption
{
    None = 0,
    ListSize = 1 << 0,
    ListLabel = 1 << 1,
    ElementLabels = 1 << 2,
    Buttons = 1 << 3,
    Default = ListSize | ListLabel | ElementLabels,
    NoElementLabels = ListSize | ListLabel,
    All = Default | Buttons
}