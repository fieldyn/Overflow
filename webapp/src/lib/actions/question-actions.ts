'use server';

import { fetchClient } from "../fetchClient";
import { Question } from "../types";

export async function getQuestions(tag?: string) {
    let url = '/questions';
    if (tag) url += '?tag=' + tag;
    return fetchClient<Question[]>(url, 'GET');
}

export async function getQuestionsById(id: string) {
    return fetchClient<Question>(`/questions/${id}`, 'GET');
}