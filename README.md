FastColoredTextBox
==================

Kawasaki IDE fork
-----------------

This is a **modified** version of FastColoredTextBox, maintained for use in the
Kawasaki IDE (K-IDE). It is based on the original component by Pavel Torgashov and
remains licensed under the GNU Lesser General Public License v3 (LGPLv3), the same
license as the upstream project (see `license.txt`).

Changes made in this fork, relative to the imported upstream baseline (commit "Init"):

- 2024-09-17 &mdash; Reworked CJK character handling.
- 2024-09-18 &mdash; Added support for CJK characters and related fixes.
- 2024-09-19 &mdash; Style changes.

Affected source files: `FastColoredTextBox/FastColoredTextBox.cs`, `FastColoredTextBox/Style.cs`.

The assembly version is unchanged from upstream (2.16.26.0); the modifications above
are the distinguishing changes of this fork.

Original project by Pavel Torgashov:
http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

---

Fast Colored TextBox is text editor component for .NET.
Allows you to create custom text editor with syntax highlighting.
It works well with small, medium, large and very-very large files.

It has such settings as foreground color, font style, background color which can be adjusted for arbitrarily selected text symbols. One can easily gain access to a text with the use of regular expressions. WordWrap, Find/Replace, Code folding and multilevel Undo/Redo are supported as well. 

![Fast Colored TextBox](http://www.codeproject.com/KB/edit/FastColoredTextBox_/fastcoloredtextbox2.png)

More details http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

Nuget package https://www.nuget.org/packages/FCTB/
