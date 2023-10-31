export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderPhotoUrl: string
    recepientId: number
    recepientUsername: string
    recepientPhotoUrl: string
    content: string
    messageSent: Date
    dateRead: Date
  }