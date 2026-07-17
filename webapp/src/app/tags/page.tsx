import { getTags } from "@/lib/actions/tag-actions";
import TagCard from "./TagCard";
import TagsHeader from "./TagsHeader";

export default async function TagsPage() {
  const {data: tags, error} = await getTags();

  if (error || !tags) throw new Error(error?.message || 'Failed to load tags');

  return (
    <div className="flex flex-col px-6">
      <TagsHeader />
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 pb-6">
        {tags.map(tag => (
          <TagCard key={tag.id} tag={tag} />
        ))}
      </div>
    </div>
  );
}
