import tinycolor from "tinycolor2";

function generateBackgroundColor(num: number) {
    return `hsl(${num * 137.508},50%,75%)`;
}

function generateForegroundColor(bgColor: string) {
    return tinycolor(bgColor).getLuminance() > 0.179 ? '#000' : '#fff';
}

export function getColorPair(num: number) {
    const background = generateBackgroundColor(num);
    const foreground = generateForegroundColor(background);

    return {backgroundColor: background, foregroundColor: foreground} as IColorPair;
}

interface IColorPair {
    backgroundColor: string,
    foregroundColor: string
}