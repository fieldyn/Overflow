import { getQuestions } from "@/lib/actions/question-actions"
import QuestionCard from "./QuestionCard";
import QuestionsHeader from "./QuestionsHeader";

export default async function QuestionsPage({searchParams}:{searchParams?:Promise<{tag?:string}>}) {
    const params = await searchParams;
    const {data: questions, error} = await getQuestions(params?.tag);

    if (error || !questions) throw new Error(error?.message || 'Failed to load questions');
  return (
    <>
    <QuestionsHeader total={questions.length} tag={params?.tag} />
        {questions.map(q => (
            <div key={q.id} className="py-4 not-last:border-b w-full flex">
                <QuestionCard key={q.id} question={q}/>
            </div>
        ))}
    </>
  )
}
