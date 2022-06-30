declare module '*.vue' {
	import { DefineComponent } from 'vue'
	const component: DefineComponent<{}, {}, any>
	export default component
}

declare module '*.svg'
declare module '*.png'
declare module '*.jpg'
declare module '*.jpeg'
declare module '*.gif'
declare module '*.bmp'
declare module '*.tiff'
declare module '*.json'

interface Object<T> {
	[key: string]: T
}

interface AnyObject extends Object<any> {
}

interface SelectObject<TValue> {
	name: string,
	value: TValue
}

function nameof<T>(obj: T, expression: (x: { [Property in keyof T]: () => string }) => () => string): string
{
    const res: { [Property in keyof T]: () => string } = {} as { [Property in keyof T]: () => string };

    Object.keys(obj).map(k => res[k as keyof T] = () => k);

    return expression(res)();
}