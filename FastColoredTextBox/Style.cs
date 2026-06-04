//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED.
//
//  License: GNU Lesser General Public License (LGPLv3)
//  Copyright (C) Pavel Torgashov, 2011-2016.
//
//  ---------------------------------------------------------------------------
//  Modified for the Kawasaki IDE (K-IDE), 2024-09-19:
//    style changes related to CJK character rendering.
//  Corresponding modified-library source:
//    https://github.com/cquante/FastColoredTextBox-kide
//  ---------------------------------------------------------------------------
//

using System.Drawing;
using System;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FastColoredTextBoxNS
{
  /// <summary>
  /// Style of chars
  /// </summary>
  /// <remarks>This is base class for all text and design renderers</remarks>
  public abstract class Style : IDisposable
  {
    /// <summary>
    /// This style is exported to outer formats (HTML for example)
    /// </summary>
    public virtual bool IsExportable { get; set; }
    /// <summary>
    /// Occurs when user click on StyleVisualMarker joined to this style 
    /// </summary>
    public event EventHandler<VisualMarkerEventArgs> VisualMarkerClick;

    /// <summary>
    /// Constructor
    /// </summary>
    public Style ()
    {
      IsExportable = true;
    }

    /// <summary>
    /// Renders given range of text
    /// </summary>
    /// <param name="gr">Graphics object</param>
    /// <param name="position">Position of the range in absolute control coordinates</param>
    /// <param name="range">Rendering range of text</param>
    public abstract void Draw (Graphics gr, Point position, Range range);

    /// <summary>
    /// Occurs when user click on StyleVisualMarker joined to this style 
    /// </summary>
    public virtual void OnVisualMarkerClick (FastColoredTextBox tb, VisualMarkerEventArgs args)
    {
      if (VisualMarkerClick != null)
        VisualMarkerClick (tb, args);
    }

    /// <summary>
    /// Shows VisualMarker
    /// Call this method in Draw method, when you need to show VisualMarker for your style
    /// </summary>
    protected virtual void AddVisualMarker (FastColoredTextBox tb, StyleVisualMarker marker)
    {
      tb.AddVisualMarker (marker);
    }

    public static Size GetSizeOfRange (Range range)
    {
      Line line = range.tb[range.Start.iLine];

      int x = 0;
      for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
        if (TextStyle.IsCJKCharacter (line[i].c))
          x += 2 * range.tb.CharWidth;
        else
          x += range.tb.CharWidth;

      //return new Size ((range.End.iChar - range.Start.iChar) * range.tb.CharWidth, range.tb.CharHeight);
      return new Size (x, range.tb.CharHeight);
    }

    public static GraphicsPath GetRoundedRectangle (Rectangle rect, int d)
    {
      GraphicsPath gp = new GraphicsPath();

      gp.AddArc (rect.X, rect.Y, d, d, 180, 90);
      gp.AddArc (rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
      gp.AddArc (rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
      gp.AddArc (rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
      gp.AddLine (rect.X, rect.Y + rect.Height - d, rect.X, rect.Y + d / 2);

      return gp;
    }

    public virtual void Dispose ()
    {
      ;
    }

    /// <summary>
    /// Returns CSS for export to HTML
    /// </summary>
    /// <returns></returns>
    public virtual string GetCSS ()
    {
      return "";
    }

    /// <summary>
    /// Returns RTF descriptor for export to RTF
    /// </summary>
    /// <returns></returns>
    public virtual RTFStyleDescriptor GetRTF ()
    {
      return new RTFStyleDescriptor ();
    }
  }

  /// <summary>
  /// Style for chars rendering
  /// This renderer can draws chars, with defined fore and back colors
  /// </summary>
  public class TextStyle : Style
  {
    public Brush ForeBrush { get; set; }
    public Brush BackgroundBrush { get; set; }
    public FontStyle FontStyle { get; set; }
    //public readonly Font Font;
    public StringFormat stringFormat;

    public TextStyle (Brush foreBrush, Brush backgroundBrush, FontStyle fontStyle)
    {
      this.ForeBrush = foreBrush;
      this.BackgroundBrush = backgroundBrush;
      this.FontStyle = fontStyle;
      stringFormat = new StringFormat (StringFormatFlags.MeasureTrailingSpaces);
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      Line line;

      //draw background
      if (BackgroundBrush != null)
      {
        line = range.tb[range.Start.iLine];

        int x = 0;
        for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          if (IsCJKCharacter (line[i].c))
            x += 2 * range.tb.CharWidth;
          else
            x += range.tb.CharWidth;

        gr.FillRectangle (BackgroundBrush, position.X, position.Y,
                          x, range.tb.CharHeight);
      }      
      //draw chars
      using (var f = new Font (range.tb.Font, FontStyle))
      {
        line = range.tb[range.Start.iLine];
        float dx = range.tb.CharWidth;
        float y = position.Y + range.tb.LineInterval/2;
        float x = position.X - range.tb.CharWidth/3;

        if (ForeBrush == null)
          ForeBrush = new SolidBrush (range.tb.ForeColor);

        if (/*range.tb.ImeAllowed*/true)
        {
          //IME mode
          for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          {
            if (IsCJKCharacter (line[i].c))
            {
              //SizeF size = FastColoredTextBox.GetCharSize(f, line[i].c);

              //var gs = gr.Save();
              //float k = size.Width > range.tb.CharWidth + 1 ? range.tb.CharWidth/size.Width : 1;
              //gr.TranslateTransform (x, y + (1 - k) * range.tb.CharHeight / 2);
              //gr.ScaleTransform (k, (float)Math.Sqrt (k));
              //gr.DrawString (line[i].c.ToString (), f, ForeBrush, 0, 0, stringFormat);
              //gr.Restore (gs);
              //x += dx;
              gr.DrawString (line[i].c.ToString (), f, ForeBrush, x, y, stringFormat);
              x += 2 * range.tb.CharWidth;
              //x += GetCharacterWidth (gr, f, line[i].c, range.tb.CharWidth);
            }
            else
            {
              gr.DrawString (line[i].c.ToString (), f, ForeBrush, x, y, stringFormat);
              x += dx;
            } // if
          }
        }
        else
        {
          //classic mode 
          for (int i = range.Start.iChar; i < range.End.iChar; i++)
          {
            //draw char
            gr.DrawString (line[i].c.ToString (), f, ForeBrush, x, y, stringFormat);
            x += dx;
          }
        }
      }
    }

    public static bool IsCJKCharacter (char c)
    {
      int codePoint = Convert.ToInt32(c);

      // CJK Unified Ideographs
      if (codePoint >= 0x4E00 && codePoint <= 0x9FFF) 
        return true;

      // CJK Unified Ideographs Extension A
      if (codePoint >= 0x3400 && codePoint <= 0x4DBF) 
        return true;

      // CJK Unified Ideographs Extension B
      if (codePoint >= 0x20000 && codePoint <= 0x2A6DF) 
        return true;

      // CJK Unified Ideographs Extension C
      if (codePoint >= 0x2A700 && codePoint <= 0x2B73F) 
        return true;

      // CJK Unified Ideographs Extension D
      if (codePoint >= 0x2B740 && codePoint <= 0x2B81F) 
        return true;

      // CJK Unified Ideographs Extension E
      if (codePoint >= 0x2B820 && codePoint <= 0x2CEAF) 
        return true;

      // CJK Unified Ideographs Extension F
      if (codePoint >= 0x2CEB0 && codePoint <= 0x2EBEF) 
        return true;

      // CJK Unified Ideographs Extension G
      if (codePoint >= 0x30000 && codePoint <= 0x3134F) 
        return true;

      // CJK Symbols and Punctuation
      if (codePoint >= 0x3000 && codePoint <= 0x303F) 
        return true;

      // CJK Strokes
      if (codePoint >= 0x31C0 && codePoint <= 0x31EF) 
        return true;

      // Enclosed CJK Letters and Months
      if (codePoint >= 0x3200 && codePoint <= 0x32FF) 
        return true;

      // CJK Compatibility
      if (codePoint >= 0x3300 && codePoint <= 0x33FF) 
        return true;

      // CJK Compatibility Ideographs
      if (codePoint >= 0xF900 && codePoint <= 0xFAFF) 
        return true;

      // CJK Compatibility Ideographs Supplement
      if (codePoint >= 0x2F800 && codePoint <= 0x2FA1F) 
        return true;

      // Hiragana
      if (codePoint >= 0x3040 && codePoint <= 0x309F) 
        return true;

      // Katakana
      if (codePoint >= 0x30A0 && codePoint <= 0x30FF) 
        return true;

      // Katakana Phonetic Extensions
      if (codePoint >= 0x31F0 && codePoint <= 0x31FF) 
        return true;

      // Hangul Jamo
      if (codePoint >= 0x1100 && codePoint <= 0x11FF) 
        return true;

      // Hangul Compatibility Jamo
      if (codePoint >= 0x3130 && codePoint <= 0x318F) 
        return true;

      // Hangul Syllables
      if (codePoint >= 0xAC00 && codePoint <= 0xD7AF) 
        return true;

      // half-size Katakana
      if (codePoint >= 0xFF65 && codePoint <= 0xFF9F)
        return false;

      // Halfwidth and Fullwidth Forms (for compatibility)
      if (codePoint >= 0xFF00 && codePoint <= 0xFFEF) 
        return true;

      return false;
    }

    public static int GetCharacterWidth (Graphics gr, Font fnt, char c, int def_char_width)
    {
      TextFormatFlags flags = TextFormatFlags.NoPadding;
      Size max_size = new Size (int.MaxValue, int.MaxValue);
      int width = TextRenderer.MeasureText (gr, c.ToString (), fnt, max_size, flags).Width + 3;

      if (width < def_char_width)
        return def_char_width;
      else
        return width;
    } // GetCharacterWidth

    public override string GetCSS ()
    {
      string result = "";

      if (BackgroundBrush is SolidBrush)
      {
        var s =  ExportToHTML.GetColorAsString((BackgroundBrush as SolidBrush).Color);
        if (s != "")
          result += "background-color:" + s + ";";
      }
      if (ForeBrush is SolidBrush)
      {
        var s = ExportToHTML.GetColorAsString((ForeBrush as SolidBrush).Color);
        if (s != "")
          result += "color:" + s + ";";
      }
      if ((FontStyle & FontStyle.Bold) != 0)
        result += "font-weight:bold;";
      if ((FontStyle & FontStyle.Italic) != 0)
        result += "font-style:oblique;";
      if ((FontStyle & FontStyle.Strikeout) != 0)
        result += "text-decoration:line-through;";
      if ((FontStyle & FontStyle.Underline) != 0)
        result += "text-decoration:underline;";

      return result;
    }

    public override RTFStyleDescriptor GetRTF ()
    {
      var result = new RTFStyleDescriptor();

      if (BackgroundBrush is SolidBrush)
        result.BackColor = (BackgroundBrush as SolidBrush).Color;

      if (ForeBrush is SolidBrush)
        result.ForeColor = (ForeBrush as SolidBrush).Color;

      if ((FontStyle & FontStyle.Bold) != 0)
        result.AdditionalTags += @"\b";
      if ((FontStyle & FontStyle.Italic) != 0)
        result.AdditionalTags += @"\i";
      if ((FontStyle & FontStyle.Strikeout) != 0)
        result.AdditionalTags += @"\strike";
      if ((FontStyle & FontStyle.Underline) != 0)
        result.AdditionalTags += @"\ul";

      return result;
    }
  }

  /// <summary>
  /// Renderer for folded block
  /// </summary>
  public class FoldedBlockStyle : TextStyle
  {
    public FoldedBlockStyle (Brush foreBrush, Brush backgroundBrush, FontStyle fontStyle) :
        base (foreBrush, backgroundBrush, fontStyle)
    {
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      Line line;

      if (range.End.iChar > range.Start.iChar)
      {
        base.Draw (gr, position, range);

        int firstNonSpaceSymbolX = position.X;
        int x = position.X;

        //find first non space symbol
        line = range.tb[range.Start.iLine];
        for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          if (line[i].c != ' ')
            break;
          else
          {
            if (IsCJKCharacter (line[i].c))
              firstNonSpaceSymbolX += 2 * range.tb.CharWidth;
            else
              firstNonSpaceSymbolX += range.tb.CharWidth;
          }

        for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          if (IsCJKCharacter (line[i].c))
            x += 2 * range.tb.CharWidth;
          else
            x += range.tb.CharWidth;

        //create marker
        range.tb.AddVisualMarker (new FoldedAreaMarker (range.Start.iLine, 
                                                        new Rectangle (firstNonSpaceSymbolX, 
                                                        position.Y, 
                                                        x - firstNonSpaceSymbolX, range.tb.CharHeight)));
      }
      else
      {
        //draw '...'
        using (Font f = new Font (range.tb.Font, FontStyle))
          gr.DrawString ("...", f, ForeBrush, range.tb.LeftIndent, position.Y - 2);
        //create marker
        range.tb.AddVisualMarker (new FoldedAreaMarker (range.Start.iLine, new Rectangle (range.tb.LeftIndent + 2, position.Y, 2 * range.tb.CharHeight, range.tb.CharHeight)));
      }
    }
  }

  /// <summary>
  /// Renderer for selected area
  /// </summary>
  public class SelectionStyle : Style
  {
    public Brush BackgroundBrush { get; set; }
    public Brush ForegroundBrush { get; private set; }

    public override bool IsExportable
    {
      get { return false; }
      set { }
    }

    public SelectionStyle (Brush backgroundBrush, Brush foregroundBrush = null)
    {
      this.BackgroundBrush = backgroundBrush;
      this.ForegroundBrush = foregroundBrush;
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      Line line;
      int x;

      if (range.Start.iChar == range.End.iChar)
        return;

      //draw background
      if (BackgroundBrush != null)
      {
        line = range.tb[range.Start.iLine];

        x = 0;
        for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          if (TextStyle.IsCJKCharacter (line[i].c))
            x += 2 * range.tb.CharWidth;
          else
            x += range.tb.CharWidth;

        gr.SmoothingMode = SmoothingMode.None;
        var rect = new Rectangle(position.X, position.Y, x, range.tb.CharHeight);
        if (rect.Width == 0)
          return;
        gr.FillRectangle (BackgroundBrush, rect);
        //
        if (ForegroundBrush != null)
        {
          //draw text
          gr.SmoothingMode = SmoothingMode.AntiAlias;

          var r = new Range(range.tb, range.Start.iChar, range.Start.iLine,
                                      Math.Min(range.tb[range.End.iLine].Count, range.End.iChar), range.End.iLine);
          using (var style = new TextStyle (ForegroundBrush, null, FontStyle.Regular))
            style.Draw (gr, new Point (position.X, position.Y - 1), r);
        }
      }
    }
  }

  /// <summary>
  /// Marker style
  /// Draws background color for text
  /// </summary>
  public class MarkerStyle : Style
  {
    public Brush BackgroundBrush { get; set; }

    public MarkerStyle (Brush backgroundBrush)
    {
      this.BackgroundBrush = backgroundBrush;
      IsExportable = true;
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      Line line;

      //draw background
      if (BackgroundBrush != null)
      {
        line = range.tb[range.Start.iLine];

        int x = 0;
        for (int i = range.Start.iChar; i < range.End.iChar && i < line.Count; i++)
          if (TextStyle.IsCJKCharacter (line[i].c))
            x += 2 * range.tb.CharWidth;
          else
            x += range.tb.CharWidth;

        Rectangle rect = new Rectangle(position.X, position.Y, x, range.tb.CharHeight);
        if (rect.Width == 0)
          return;
        gr.FillRectangle (BackgroundBrush, rect);
      }
    }

    public override string GetCSS ()
    {
      string result = "";

      if (BackgroundBrush is SolidBrush)
      {
        var s = ExportToHTML.GetColorAsString((BackgroundBrush as SolidBrush).Color);
        if (s != "")
          result += "background-color:" + s + ";";
      }

      return result;
    }
  }

  /// <summary>
  /// Draws small rectangle for popup menu
  /// </summary>
  public class ShortcutStyle : Style
  {
    public Pen borderPen;

    public ShortcutStyle (Pen borderPen)
    {
      this.borderPen = borderPen;
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      //get last char coordinates
      Point p = range.tb.PlaceToPoint(range.End);
      //draw small square under char
      Rectangle rect = new Rectangle(p.X - 5, p.Y + range.tb.CharHeight - 2, 4, 3);
      gr.FillPath (Brushes.White, GetRoundedRectangle (rect, 1));
      gr.DrawPath (borderPen, GetRoundedRectangle (rect, 1));
      //add visual marker for handle mouse events
      AddVisualMarker (range.tb, new StyleVisualMarker (new Rectangle (p.X - range.tb.CharWidth, p.Y, range.tb.CharWidth, range.tb.CharHeight), this));
    }
  }

  /// <summary>
  /// This style draws a wavy line below a given text range.
  /// </summary>
  /// <remarks>Thanks for Yallie</remarks>
  public class WavyLineStyle : Style
  {
    private Pen Pen { get; set; }

    public WavyLineStyle (int alpha, Color color)
    {
      Pen = new Pen (Color.FromArgb (alpha, color));
    }

    public override void Draw (Graphics gr, Point pos, Range range)
    {
      var size = GetSizeOfRange(range);
      var start = new Point(pos.X, pos.Y + size.Height - 1);
      var end = new Point(pos.X + size.Width, pos.Y + size.Height - 1);
      DrawWavyLine (gr, start, end);
    }

    private void DrawWavyLine (Graphics graphics, Point start, Point end)
    {
      if (end.X - start.X < 2)
      {
        graphics.DrawLine (Pen, start, end);
        return;
      }

      var offset = -1;
      var points = new List<Point>();

      for (int i = start.X; i <= end.X; i += 2)
      {
        points.Add (new Point (i, start.Y + offset));
        offset = -offset;
      }

      graphics.DrawLines (Pen, points.ToArray ());
    }

    public override void Dispose ()
    {
      base.Dispose ();

      if (Pen != null)
        Pen.Dispose ();
    }
  }

  /// <summary>
  /// This style is used to mark range of text as ReadOnly block
  /// </summary>
  /// <remarks>You can inherite this style to add visual effects of readonly text</remarks>
  public class ReadOnlyStyle : Style
  {
    public ReadOnlyStyle ()
    {
      IsExportable = false;
    }

    public override void Draw (Graphics gr, Point position, Range range)
    {
      //
    }
  }
}
