'use server';

import { fetchClient } from "../fetchClient";
import { Question } from "../types";

export async function getQuestions(tag?: string) : Promise<Question[]> {
    let url = '/questions';
    if (tag) url += '?tag=' + tag;
    return fetchClient<Question[]>(url, 'GET');
}

export async function getQuestionsById(id: string) : Promise<Question> {
    return fetchClient<Question>(`/questions/${id}`, 'GET');
}