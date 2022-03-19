export interface Message {
  id: number
  senderId: number
  senderPhotoUrl: string
  recipientId: number
  senderUsername: string
  recipientUsername: string
  recipientPhotoUrl: string
  content: string
  dateRead?: Date
  messageSent: Date
}
