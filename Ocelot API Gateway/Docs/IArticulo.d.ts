export interface IArticulo {
    datosBasicos: DatosBasicos;
    datosEconomicos: DatosEconomicos;
    imagen: string;
    datosGestion: DatosGestion;
}

export interface DatosBasicos {
    codigoBarras: string;
    referencia: string;
    nombre: string;
    familia: string[];
    proveedor: string;
    ubicacion: string;
    fechaAlta: string;
}

export interface DatosEconomicos {
    coste: number;
    margenBeneficio: number[];
    iva: number[];
}

export interface DatosGestion {
    stock: Stock;
    unidadesCaja: number;
}

export interface Stock {
    actual: number;
    minimo: number;
}

