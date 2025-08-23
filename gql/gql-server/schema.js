const {gql} = require('apollo-server')

module.exports = gql`
    type Author {
  id: ID!
  name: String!
  posts: [Post!]!
}

type Post {
  id: ID!
  title: String!
  content: String!
  author: Author!
}

type Query {
  posts: [Post!]!
  author(id: ID!): Author
}

type Mutation {
  createPost(title: String!, content: String!, authorId: ID!): Post!
}
`;