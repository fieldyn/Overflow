import { getQuestions } from "@/lib/actions/question-actions"

export default async function QuestionsPage() {
    const questions = await getQuestions();
  return (
    <div>
      <ul>
        {questions.map(q => (
            <li key={q.id}>{q.title}</li>
        ))}
      </ul>
    </div>
  )
}
