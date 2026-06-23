import { getQuestions } from "@/lib/actions/question-actions"
import QuestionCard from "./QuestionCard";

export default async function QuestionsPage() {
    const questions = await getQuestions();
  return (
    <>
        {questions.map(q => (
            <div key={q.id} className="py-4 not-last:border-b w-full flex">
                <QuestionCard key={q.id} question={q}/>
            </div>
        ))}
    </>
  )
}
