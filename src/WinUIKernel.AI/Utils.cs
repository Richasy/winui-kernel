// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace Richasy.WinUIKernel.AI;

internal static class Utils
{
    public static Brush GetProviderBackgroundBrush(string provider)
    {
        switch (provider.ToLowerInvariant())
        {
            case "anthropic":
                return new SolidColorBrush("#FAF9F5".ToColor());
            case "azureopenai" or "azureai" or "xai":
                return new SolidColorBrush("#FFFFFF".ToColor());
            case "qwen":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(1, 1),
                    GradientStops =
                    [
                        new GradientStop{ Color = "#00055F".ToColor(), Offset = 0 },
                        new GradientStop{ Color = "#6F69F7".ToColor(), Offset = 1 },
                    ]
                };
            case "deepseek":
                return new SolidColorBrush("#4D6BFE".ToColor());
            case "gemini":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 1),
                    EndPoint = new(1, 0),
                    GradientStops =
                    [
                        new GradientStop { Color = "#1C69FF".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#F0DCD6".ToColor(), Offset = 1 },
                    ]
                };
            case "groq":
                return new SolidColorBrush("#F55036".ToColor());
            case "hunyuan":
                return new SolidColorBrush("#0053e0".ToColor());
            case "lingyi":
                return new SolidColorBrush("#003425".ToColor());
            case "mistralai":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(0, 1),
                    GradientStops =
                    [
                        new GradientStop { Color = "#F7D046".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#F2A73B".ToColor(), Offset = 0.25 },
                        new GradientStop { Color = "#EE792F".ToColor(), Offset = 0.5 },
                        new GradientStop { Color = "#EE792F".ToColor(), Offset = 0.75 },
                        new GradientStop { Color = "#EA3326".ToColor(), Offset = 1 },
                    ]
                };
            case "moonshot":
                return new SolidColorBrush("#16191E".ToColor());
            case "ollama":
                return new SolidColorBrush(Colors.White);
            case "openai":
                return new SolidColorBrush(Colors.Black);
            case "openrouter":
                return new SolidColorBrush("#6566F1".ToColor());
            case "perplexity":
                return new SolidColorBrush("#22B8CD".ToColor());
            case "ernie":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(1, 0),
                    GradientStops =
                    [
                        new GradientStop { Color = "#0A51C3".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#23A4FB".ToColor(), Offset = 1 },
                    ]
                };
            case "spark":
                return new SolidColorBrush("#0070f0".ToColor());
            case "togetherai":
                return new SolidColorBrush("#0f6fff".ToColor());
            case "zhipu":
                return new SolidColorBrush("#3859FF".ToColor());
            case "baidu":
                return new SolidColorBrush("#2932E1".ToColor());
            case "azure":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(1, 0),
                    GradientStops =
                    [
                        new GradientStop { Color = "#3CCBF4".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#2892DF".ToColor(), Offset = 1 },
                    ]
                };
            case "azurespeech":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(1, 0),
                    GradientStops =
                    [
                        new GradientStop { Color = "#50E6FF".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#773ADC".ToColor(), Offset = 1 },
                    ]
                };
            case "ali":
                return new SolidColorBrush("#FF6A00".ToColor());
            case "tencent":
                return new SolidColorBrush("#00A3FF".ToColor());
            case "youdao":
                return new SolidColorBrush("#E10012".ToColor());
            case "volcano":
                return new LinearGradientBrush
                {
                    StartPoint = new(0, 0),
                    EndPoint = new(1, 0),
                    GradientStops =
                    [
                        new GradientStop { Color = "#00F3CE".ToColor(), Offset = 0 },
                        new GradientStop { Color = "#3F30FF".ToColor(), Offset = 1 },
                    ]
                };
            case "siliconflow":
                return new SolidColorBrush("#7C3AED".ToColor());
            case "doubao":
            case "windows":
            case "google":
            case "edge":
                return new SolidColorBrush(Colors.White);
            default:
                return new SolidColorBrush(Colors.Transparent);
        }
    }
}
