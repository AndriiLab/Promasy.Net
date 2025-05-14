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

interface Object<Tkey, Tvalue> {
	[key: Tkey]: Tvalue
}

interface Object<T> {
	[key: string]: T
}

interface AnyObject extends Object<any> {
}

interface SelectObject<TValue> {
	name: string,
	value: TValue
}

interface TablePagingData {
	page: number;
	offset: number;
	filter?: string;
	orderBy?: string;
	descending?: boolean;
	total: number;
}