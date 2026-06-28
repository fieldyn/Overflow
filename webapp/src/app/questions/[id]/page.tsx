import { getQuestionsById } from "@/lib/actions/question-actions";
import QuestionDetailedHeader from "./QuestionDetailedHeader";
import QuestionContent from "./QuestionContent";
import { notFound } from "next/navigation";

type Params = Promise<{id: string}>

export default async function QuestionDetailedPage({params}: {params: Params}) {
    const id = (await params).id;
    const question = await getQuestionsById(id);

    if(!question) return notFound();
    
  return (
    <div>
      <QuestionDetailedHeader question={question} />
      <QuestionContent question={question} />
    </div>
  )
}
