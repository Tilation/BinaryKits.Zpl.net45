﻿using BinaryKits.Zpl.Label.Elements;
using BinaryKits.Zpl.Viewer.Helpers;
using SkiaSharp;
using System;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    public class TextFieldElementDrawer : ElementDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element.GetType() == typeof(ZplTextField);
        }

        public override bool IsReverseDraw(ZplElementBase element)
        {
            if (element is ZplTextField textField)
            {
                return textField.ReversePrint;
            }

            return false;
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element)
        {
            Draw(element, new DrawerOptions());
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element, DrawerOptions options)
        {
            if (element is ZplTextField textField)
            {
                float x = textField.PositionX;
                float y = textField.PositionY;

                var font = textField.Font;

                float fontSize = font.FontHeight > 0 ? font.FontHeight : font.FontWidth;
                var scaleX = 1.0f;
                if (font.FontWidth != 0 && font.FontWidth != fontSize)
                {
                    scaleX = (float)font.FontWidth / fontSize;
                }

                fontSize *= 0.95f;

                var typeface = options.FontLoader(font.FontName);

                using var skPaint = new SKPaint();
                skPaint.Color = SKColors.Black;
                skPaint.Typeface = typeface;
                skPaint.TextSize = fontSize;
                skPaint.TextScaleX = scaleX;

                var textBounds = new SKRect();
                var textBoundBaseline = new SKRect();
                string DisplayText = textField.Text.ReplaceSpecialChars();
                skPaint.MeasureText(new string('A', DisplayText.Length), ref textBoundBaseline);
                skPaint.MeasureText(DisplayText, ref textBounds);

                if (textField.FieldTypeset != null)
                {
                    y -= textBounds.Height;
                }

                using (new SKAutoCanvasRestore(this._skCanvas))
                {
                    SKMatrix matrix = SKMatrix.Empty;

                    if (textField.FieldOrigin != null)
                    {
                        switch (textField.Font.FieldOrientation)
                        {
                            case Label.FieldOrientation.Rotated90:
                                matrix = SKMatrix.CreateRotationDegrees(90, x, y);
                                y -= font.FontHeight - textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Rotated180:
                                matrix = SKMatrix.CreateRotationDegrees(180, x, y);
                                x -= textBounds.Width;
                                y -= font.FontHeight - textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Rotated270:
                                matrix = SKMatrix.CreateRotationDegrees(270, x, y);
                                x -= textBounds.Width;
                                y += textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Normal:
                                y += textBoundBaseline.Height;
                                break;
                        }
                    }
                    else
                    {
                        switch (textField.Font.FieldOrientation)
                        {
                            case Label.FieldOrientation.Rotated90:
                                matrix = SKMatrix.CreateRotationDegrees(90, x, y);
                                x += textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Rotated180:
                                matrix = SKMatrix.CreateRotationDegrees(180, x, y);
                                y -= textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Rotated270:
                                matrix = SKMatrix.CreateRotationDegrees(270, x, y);
                                x -= textBoundBaseline.Height;
                                break;
                            case Label.FieldOrientation.Normal:
                                y += textBoundBaseline.Height;
                                break;
                        }
                    }

                    if (matrix != SKMatrix.Empty)
                    {
                        this._skCanvas.SetMatrix(matrix);
                    }

                    this._skCanvas.DrawText(DisplayText, x, y, skPaint);
                }
            }
        }
    }
}
