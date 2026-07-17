'use server';

import { fetchClient } from "../fetchClient";

export async function getError(code: number) {
    return fetchClient(`/questions/errors?code=${code}`, 'GET');
}
