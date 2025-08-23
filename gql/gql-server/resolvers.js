// resolvers.js
// A simple mock database for our example
const posts = [
  { id: '1', title: 'First Post', content: 'This is the first post.' },
  { id: '2', title: 'Second Post', content: 'This is the second post.' },
];

const resolvers = {
  Query: {
    posts: () => posts,
  },
  Mutation: {
    createPost: (_, { title, content }) => {
      const newPost = {
        id: String(posts.length + 1),
        title,
        content,
      };
      posts.push(newPost);
      return newPost;
    },
  },
};

module.exports = resolvers;